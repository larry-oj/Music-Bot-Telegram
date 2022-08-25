using Music_Bot_Telegram.Data;
using Music_Bot_Telegram.Services.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Music_Bot_Telegram.Services.Commands;

public class Search : ICommand
{
    public string Name 
        => "search";
    
    public string Description 
        => "Super secret command";

    public bool IsAdmin 
        => false;
    
    private readonly IMyApiService _api;
    private readonly BotDbContext _context;

    public Search()
    {
    }
    
    public Search(
        IMyApiService api, 
        BotDbContext context)
    {
        _api = api;
        _context = context;
    }
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        var stage = user.SessionStage switch
        {
            // ask user for search query
            1 => StageOne(botClient, message, user),
            
            // provide user with results
            2 => StageTwo(botClient, message, user),
            
            // ask user for a platform
            _ => StageZero(botClient, message, user),
        };

        await stage;
        await _context.SaveChangesAsync();
    }
    
    private async Task StageZero(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("YouTube"),
                new KeyboardButton("Spotify")
            },
            new[]
            {
                new KeyboardButton("/cancel")
            }
        });
        
        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Choose a platform:",
            replyMarkup: keyboard);
        
        user.IsActiveSession = true;
        user.SessionCommand = Name;
        user.SessionStage = 1;
    }
    
    private async Task StageOne(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("/cancel")
            }
        });
        
        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Enter search query:",
            replyMarkup: keyboard);
        
        user.SessionData = message.Text!.ToLower();
        user.SessionStage = 2;
    }
    
    private async Task StageTwo(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        var text = "";
        var imageUrl = "";
        
        switch (user.SessionData)
        {
            case "youtube":
                var ytData = await _api.SearchYoutubeAsync(message.Text!);
                text = ytData.ToString();
                imageUrl = ytData.Items.First().Snippet.Thumbnails.High.Url;
                break;
            
            case "spotify":
                var spData = await _api.SearchSpotifyAsync(message.Text!);
                text = spData.ToString();
                imageUrl = spData.Tracks.First().Album.Covers.First().Url;
                break;
            
            default:
                throw new ArgumentException("Invalid platform provided");
                break;
        }

        await botClient.SendPhotoAsync(
            chatId: message.Chat.Id,
            photo: imageUrl,
            caption: text,
            replyMarkup: new ReplyKeyboardRemove());

        user.IsActiveSession = false;
        user.SessionCommand = null;
        user.SessionData = null;
        user.SessionStage = null;
    }
}