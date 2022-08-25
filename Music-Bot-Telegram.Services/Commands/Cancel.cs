using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Music_Bot_Telegram.Services.Commands;

public class Cancel : ICommand
{
    public string Name 
        => "cancel";
    
    public string Description 
        => "Cancels current operation";

    public bool IsAdmin 
        => false;

    private readonly BotDbContext _context;
    
    public Cancel()
    {
    }

    public Cancel(BotDbContext context)
    {
        _context = context;
    }
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, Data.Models.User user)
    {
        user.IsActiveSession = false;
        user.SessionCommand = null;
        user.SessionData = null;
        user.SessionStage = null;
        await _context.SaveChangesAsync();
        
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id, 
            text: "Cancelled successfully",
            replyMarkup: new ReplyKeyboardRemove());
    }
}