using Music_Bot_Telegram.Data.Models;

namespace Music_Bot_Telegram.Data;

public interface IUnitOfWork : IDisposable
{
    Repo<User> Users { get; }
    Repo<Models.Action> Actions { get; }
    Repo<ActionType> ActionTypes { get; }
    
    void Save();
    Task SaveAsync();
}