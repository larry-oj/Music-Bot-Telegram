using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Music_Bot_Telegram.Data.Models.User;

namespace Music_Bot_Telegram.Services.Commands;

public class Recognize : ICommand
{
    public string Name 
        => "recognize";
    
    public string Description 
        => "Recognizes a song from voice / video / audio message";

    public bool IsAdmin 
        => false;
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, User user)
    {
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Note: You don't need to use /recognize command to use this functionality. Just send me media at any time!\n\nDescription:\nSend me a voice / audio / video / videonote message and I will try to recognize a song from it!\n\nLimitations:\nLimited to 34 request per day across all users. Recognition provider is not free and therefore, to keep the bot free, the amount of request is limited");
    }
}