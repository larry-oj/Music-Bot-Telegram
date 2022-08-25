using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Music_Bot_Telegram.Services.Commands;

public class Download : ICommand
{
    public string Name 
        => "download";
    
    public string Description 
        => "Downloads a song from YouTube / Spotify";

    public bool IsAdmin 
        => false;

    private readonly IMyApiService _myApiService;
    private readonly BotDbContext _context;
    
    public Download()
    {
    }

    public Download(
        IMyApiService myApiService,
        BotDbContext context)
    {
        _myApiService = myApiService;
        _context = context;
    }
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        if (user.IsActiveConversion) 
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "You already have an active request!");
            return;
        }
        
        var stage = user.SessionStage switch
        {
            // start conversion service
            1 => StageOne(botClient, message, user),
            
            // ask user for a link
            _ => StageZero(botClient, message, user)
        };

        await stage;
        await _context.SaveChangesAsync();
    }

    private async Task StageZero(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        user.IsActiveSession = true;
        user.SessionCommand = Name;
        user.SessionStage = 1;

        await botClient.SendTextMessageAsync(
            chatId: message.Chat!.Id,
            text: "Send me a Spotify / YouTube link\nUse /cancel to cancel");
    }

    private async Task StageOne(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        user.IsActiveSession = false;
        user.SessionCommand = null;
        user.SessionStage = null;
        user.IsActiveConversion = true;

        await botClient.SendTextMessageAsync(
            chatId: message.Chat!.Id,
            text: "Enqueueing...");
        
        var apiResponse = await _myApiService.EnqueueConversionAsync(message.Text!);
        user.ConversionId = apiResponse.Id;

        await botClient.SendTextMessageAsync(
            chatId: message.Chat!.Id,
            text: "Request enqueued!\nYour mp3 file will be be sent to you upon completion. Conversion usually takes between 20-120 seconds.");
    }
}