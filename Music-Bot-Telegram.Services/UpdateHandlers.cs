﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Music_Bot_Telegram.Data;
using Music_Bot_Telegram.Data.Models;
using Music_Bot_Telegram.Services.Commands;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Music_Bot_Telegram.Services;

public class UpdateHandlers
{
    private readonly ILogger<UpdateHandlers> _logger;
    private readonly IServiceProvider _serviceProvider;

    public UpdateHandlers(
        ILogger<UpdateHandlers> logger,
        IServiceProvider serviceProvider) => 
        (_logger, _serviceProvider) = (logger, serviceProvider);
    
    public Task PollingErrorHandler(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken = default)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _                                       => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update received");

        using var scope = _serviceProvider.CreateScope();
        
        var handler = update switch
        {
            { Message: { } message } => OnMessageAsync(botClient, message, scope, cancellationToken),
            _ => OnUnknownAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task OnMessageAsync(ITelegramBotClient botClient, Message message, IServiceScope scope, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(message.Text))
            return;

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        if (unitOfWork.Users.Get(message.From!.Id) is not { } user)
        {
            user = new Data.Models.User(message.From.Id);
            unitOfWork.Users.Insert(user);
            await unitOfWork.SaveAsync();
        }

        if (!user.IsActiveSession && !message.Text.StartsWith("/"))
            return;

        var commandName = user.IsActiveSession switch
        {
            true => unitOfWork.Actions.GetAll()
                .Where(u => u.User == user)
                .MaxBy(u => u.CreatedAt)!
                .Command,
            
            false => message.Text
                .Split(null)[0]
                .Replace("/", "")
                .ToLower()
        };
        
        var commands = scope.ServiceProvider.GetRequiredService<IEnumerable<ICommand>>();
        
        var command = commands.FirstOrDefault(c => c.Name == commandName);
        if (command is null)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id, 
                text: "Unknown command!", 
                cancellationToken: cancellationToken);
            
            return;
        }
        await command.ExecuteAsync(botClient, message, user);
    }
    
    private Task OnUnknownAsync(Update update, CancellationToken cancellationToken = default)
    {
        // TODO: Handle unknown / unsupported update
        return Task.CompletedTask;
    }
}