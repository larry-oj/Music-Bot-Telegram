using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Music_Bot_Telegram.Services.Commands;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    bool IsAdmin { get; }
    Task ExecuteAsync(ITelegramBotClient botClient, Message message, IUnitOfWork unitOfWork);
}