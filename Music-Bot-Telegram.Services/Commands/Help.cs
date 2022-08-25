using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Music_Bot_Telegram.Data.Models.User;

namespace Music_Bot_Telegram.Services.Commands;

public class Help : ICommand
{
    public string Name 
        => "help";
    
    public string Description 
        => "[PH] help command description";
    
    public bool IsAdmin 
        => false;
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, User user)
    {
        // cant pass from DI because of circular dependency
        var types = typeof(ICommand).Assembly.GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type) && type.IsClass);
        
        // practically a horrible crutch but that's okay 
        var @string = types
            .Select(type => (ICommand)Activator.CreateInstance(type)!)
            .Aggregate("Here is the list of all commands:\n", 
                (current, obj) => current + $"/{obj.Name} - {obj.Description}\n");

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: @string);
    }
}