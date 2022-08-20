using Microsoft.EntityFrameworkCore;
using Music_Bot_Telegram.Configuration;
using Music_Bot_Telegram.Data;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // configuration pattern
        services.Configure<TelegramConfiguration>(
            context.Configuration.GetSection(TelegramConfiguration.Configuration));

        // db
        services.AddDbContextFactory<BotDbContext>(o =>
            o.UseNpgsql(context.Configuration.GetConnectionString("Database:ConnectionString")));
            
        // services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    })
    .Build();

await host.RunAsync();