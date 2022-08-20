using Microsoft.EntityFrameworkCore;
using Music_Bot_Telegram.Data.Models;

namespace Music_Bot_Telegram.Data;

public class BotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Models.Action> Actions { get; set; }
    public DbSet<ActionType> ActionTypes { get; set; }

    // public BotDbContext(DbContextOptions<BotDbContext> options)
    //     : base(options)
    // {
    //     // ...
    // }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ...
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=musicbotdb;Username=postgres;Password=superuser");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionType>()
            .HasIndex(u => u.Name)
            .IsUnique();
    }
}