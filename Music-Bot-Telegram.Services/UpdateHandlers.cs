using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Music_Bot_Telegram.Data;
using Music_Bot_Telegram.Data.Models;
using Music_Bot_Telegram.Services.Commands;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Music_Bot_Telegram.Data.Models;

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

    private async Task<Data.Models.User?> GetValidUserAsync(BotDbContext context, Message message)
    {
        if (context.Users.FirstOrDefault(u => u.Id == message.From!.Id) is not { } user)
        {
            user = new Data.Models.User(message.From!.Id);
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        return user.IsBanned ? null : user;
    }
    
    private async Task OnMessageAsync(ITelegramBotClient botClient, Message message, IServiceScope scope, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(message.Text))
            return;

        var context = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        
        if (await GetValidUserAsync(context, message) is not { } user)
            return;

        if (!user.IsActiveSession && !message.Text.StartsWith("/"))
            return;

        var commandName = user.IsActiveSession switch
        {
            true => context.Actions
                .Where(u => u.User == user)
                .OrderByDescending(u => u.CreatedAt)
                .FirstOrDefault()?
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