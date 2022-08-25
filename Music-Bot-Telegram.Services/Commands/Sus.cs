using Telegram.Bot;
using Telegram.Bot.Types;
using User = Music_Bot_Telegram.Data.Models.User;

namespace Music_Bot_Telegram.Services.Commands;

public class Sus : ICommand
{
    public string Name 
        => "sus";
    
    public string Description 
        => "A very sussy command";

    public bool IsAdmin 
        => false;
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, User user)
    {
        var list = new string[] {"amogus", "sus", "sussy", "sussy baka", "among us", "impostor is sus"};
        
        // get random item from array
        var random = new Random();
        var randomItem = list[random.Next(list.Length)];

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: randomItem);
    }
}