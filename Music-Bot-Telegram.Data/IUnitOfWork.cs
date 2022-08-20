namespace Music_Bot_Telegram.Data;

public interface IUnitOfWork : IDisposable
{
    void Save();
    Task SaveAsync();
}