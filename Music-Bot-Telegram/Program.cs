using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Music_Bot_Telegram.Configuration;
using Music_Bot_Telegram.Data;
using Music_Bot_Telegram.Services;
using Telegram.Bot;

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
        services.AddSingleton<UpdateHandlers>();
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var o = sp.GetService<IOptions<TelegramConfiguration>>()?.Value;
            TelegramBotClientOptions options = new(o!.Token);
            return new TelegramBotClient(options);
        });
        services.AddHostedService<PollingService>();
    })
    .Build();

await host.RunAsync();