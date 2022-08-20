namespace Music_Bot_Telegram.Data;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}