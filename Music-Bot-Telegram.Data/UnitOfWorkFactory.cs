using Microsoft.EntityFrameworkCore;

namespace Music_Bot_Telegram.Data;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory<BotDbContext> _contextFactory;

    public UnitOfWorkFactory(IDbContextFactory<BotDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public IUnitOfWork Create()
        => new UnitOfWork(_contextFactory);
}