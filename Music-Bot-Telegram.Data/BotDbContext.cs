using Microsoft.EntityFrameworkCore;
using Music_Bot_Telegram.Data.Models;

namespace Music_Bot_Telegram.Data;

public class BotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RequestMeter> RequestMeters { get; set; }

    public BotDbContext(DbContextOptions<BotDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ...
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ...
    }
}