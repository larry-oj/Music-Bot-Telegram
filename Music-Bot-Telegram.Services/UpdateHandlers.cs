using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Music_Bot_Telegram.Configuration;
using Music_Bot_Telegram.Data;
using Music_Bot_Telegram.Services.Commands;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Music_Bot_Telegram.Data.Models;
using Telegram.Bot.Types.ReplyMarkups;

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
        if (message.Audio != null ||
            message.Voice != null ||
            message.Video != null ||
            message.VideoNote != null)
        {
            await OnMediaAsync(botClient, message, scope, cancellationToken);
            return;
        }
        
        if (string.IsNullOrEmpty(message.Text))
            return;

        var context = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        
        if (await GetValidUserAsync(context, message) is not { } user)
            return;

        if (!user.IsActiveSession && !message.Text.StartsWith("/"))
            return;

        var commandName = user.IsActiveSession switch
        {
            true => user.SessionCommand,
            false => message.Text
                .Split(null)[0]
                .Replace("/", "")
                .ToLower()
        };
        
        // but, if /cancel
        if (message.Text!.ToLower() == "/cancel")
        {
            commandName = "cancel";
        }
        
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

        try
        {
            await command.ExecuteAsync(botClient, message, user);
        }
        catch (Exception ex)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat!.Id, 
                text: ex.Message, 
                cancellationToken: cancellationToken);
        }
        
    }

    private async Task OnMediaAsync(ITelegramBotClient botClient, Message message, IServiceScope scope, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Recognizing...", 
            cancellationToken: cancellationToken);
        
        string fileId;
        
        if (message.Audio != null)
            fileId = message.Audio.FileId;
        else if (message.Voice != null)
            fileId = message.Voice.FileId;
        else if (message.Video != null)
            fileId = message.Video.FileId;
        else
            fileId = message.VideoNote!.FileId;

        var telegramToken = scope.ServiceProvider.GetService<IOptions<TelegramConfiguration>>();
        var file = await botClient.GetFileAsync(fileId, cancellationToken: cancellationToken);
        var api = scope.ServiceProvider.GetRequiredService<IMyApiService>();
        var data = await api.RecognizeAsync("https://api.telegram.org/file/bot" + telegramToken!.Value.Token + "/" + file.FilePath!);

        if (data == null)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Sorry! I could not recognize anything!", 
                cancellationToken: cancellationToken);
            return;
        }
        
        var song = data.Title + " - " + data.Artist;
        var ytData = await api.SearchYoutubeAsync(song);
        var spData = await api.SearchSpotifyAsync(song);
        var @string = "";
        @string += "Title: " + data.Title + "\n";
        @string += "Artist: " + data.Artist + "\n";
        @string += "Timecode: " + data.Timecode + "\n\n";
        if (ytData.Items.Count > 0)
            @string += "YouTube: https://youtu.be/" + ytData.Items.First().VideoId + "\n";
        if (spData.Tracks.Count > 0)
            @string += "Spotify: " + spData.Tracks.First().ExternalUrls.Spotify + "\n";

        await botClient.SendPhotoAsync(
            chatId: message.Chat.Id,
            photo: ytData.Items.First().Snippet.Thumbnails.High.Url,
            caption: @string, 
            cancellationToken: cancellationToken);
    }

    private Task OnUnknownAsync(Update update, CancellationToken cancellationToken = default)
    {
        // TODO: Handle unknown / unsupported update
        return Task.CompletedTask;
    }
}