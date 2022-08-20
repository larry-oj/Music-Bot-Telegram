using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Music_Bot_Telegram.Services.Commands;

public class Ping : ICommand
{
    public string Name 
        => "ping";
    
    public string Description 
        => "Super secret command";

    public bool IsAdmin 
        => false;
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, IUnitOfWork unitOfWork)
    {
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Pong!");
    }
}