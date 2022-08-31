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
        => "Searches for music on popular music providers";

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
                new KeyboardButton("Everywhere"),
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
        if (message.Text!.ToLower() != "youtube" && 
            message.Text!.ToLower() != "spotify" &&
            message.Text!.ToLower() != "everywhere")
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Invalid platform provided!\nTry again");
            await StageZero(botClient, message, user);
            return;
        }

        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Enter search query:\nUse /cancel to cancel",
            replyMarkup: new ReplyKeyboardRemove());
        
        user.SessionData = message.Text!.ToLower();
        user.SessionStage = 2;
    }
    
    private async Task StageTwo(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        var text = "";
        var imageUrl = "";
        var spPreview = "";
        var spTitle = "";
        
        switch (user.SessionData)
        {
            case "youtube":
                var ytData = await _api.SearchYoutubeAsync(message.Text!);
                if (ytData.Items.Count > 0)
                {
                    text = ytData.ToString();
                    imageUrl = ytData.Items.First().Snippet.Thumbnails.High.Url;
                }
                else
                {
                    text = "Sorry, nothing was found!";
                }
                break;
            
            case "spotify":
                var spData = await _api.SearchSpotifyAsync(message.Text!);
                if (spData.Tracks.Count > 0)
                {
                    text = spData.ToString();
                    imageUrl = spData.Tracks.First().Album.Covers.First().Url;
                    spPreview = spData.Tracks.First().PreviewUrl ?? "";
                    spTitle = spData.Tracks.First().Name;
                }
                else
                {
                    text = "Sorry, nothing was found!";
                }
                break;
            
            case "everywhere":
                ytData = await _api.SearchYoutubeAsync(message.Text!);
                spData = await _api.SearchSpotifyAsync(message.Text!);
                if (ytData.Items.Count > 0)
                {
                    text = "Youtube:\n";
                    text += "Name: " + ytData.Items.First().Snippet.Title + "\n";
                    text += "Channel: " + ytData.Items.First().Snippet.ChannelTitle + "\n";
                    text += "Link: https://youtu.be/" + ytData.Items.First().VideoId + "\n\n";
                }
                if (spData.Tracks.Count > 0)
                {
                    text += "Spotify:\n";
                    text += "Name: " + spData.Tracks.First().Name + "\n";
                    text += "Artist: " + spData.Tracks.First().Artists!.First().Name + "\n";
                    text += "Link: " + spData.Tracks.First().ExternalUrls.Spotify + "\n";
                    spPreview = spData.Tracks.First().PreviewUrl ?? "";
                    spTitle = spData.Tracks.First().Name;
                }

                if (text == "")
                {
                    text = "Sorry, nothing was found!";
                }
                else
                {
                    imageUrl = ytData.Items.First().Snippet.Thumbnails.High.Url;
                }
                break;
                
            default:
                return;
        }

        if (text == "Sorry, nothing was found!")
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text);
        }
        else
        {
            await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: imageUrl,
                caption: text);
            
            if (spPreview is not "")
            {
                await botClient.SendAudioAsync(
                    chatId: message.Chat.Id,
                    audio: spPreview!,
                    caption: "Spotify Preview");
            }
        }

        

        user.IsActiveSession = false;
        user.SessionCommand = null;
        user.SessionData = null;
        user.SessionStage = null;
    }
}