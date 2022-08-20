using Microsoft.EntityFrameworkCore;
using Music_Bot_Telegram.Data.Models;
using Action = Music_Bot_Telegram.Data.Models.Action;

namespace Music_Bot_Telegram.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly BotDbContext _context;
    private Repo<User> _userRepo;
    private Repo<Models.Action> _actionRepo;
    private Repo<ActionType> _actionTypeRepo;
    
    public Repo<User> Users 
        => _userRepo ??= new Repo<User>(_context);
    public Repo<Models.Action> Actions 
        => _actionRepo ??= new Repo<Models.Action>(_context);
    public Repo<ActionType> ActionTypes
        => _actionTypeRepo ??= new Repo<ActionType>(_context);

    public UnitOfWork(IDbContextFactory<BotDbContext> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                _context.Dispose();
        
        _disposed = true;
    }

    public void Dispose() 
        => Dispose(true);
}