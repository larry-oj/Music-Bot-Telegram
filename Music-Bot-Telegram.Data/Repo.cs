using Microsoft.EntityFrameworkCore;
using Music_Bot_Telegram.Data.Models;

namespace Music_Bot_Telegram.Data;

public class Repo<T> : IDisposable where T : class, IEntity
{
    private readonly BotDbContext _context;
    private readonly DbSet<T> _entities;

    public Repo(BotDbContext context)
    {
        _context = context;
        _entities = _context.Set<T>();
    }

    public IEnumerable<T> GetAll() 
        => _entities.AsEnumerable();

    public T? Get(long id) 
        => _entities.FirstOrDefault(e => e.Id == id);

    public void Insert(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _entities.Add(entity);
    }

    public void Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _entities.Update(entity);
    }

    public void Delete(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _entities.Remove(entity);
    }

    public void Save() 
        => _context.SaveChanges();

    public async Task SaveAsync() 
        => await _context.SaveChangesAsync();

    public void Dispose() 
        => _context.Dispose();
}