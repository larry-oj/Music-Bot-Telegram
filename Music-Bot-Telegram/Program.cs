using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Music_Bot_Telegram.Configuration;
using Music_Bot_Telegram.Data;
using Music_Bot_Telegram.Services;
using Music_Bot_Telegram.Services.Commands;
using Music_Bot_Telegram.Services.Configuration;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // configuration pattern
        services.Configure<TelegramConfiguration>(
            context.Configuration.GetSection(TelegramConfiguration.Configuration));
        services.Configure<MyApiConfiguration>(
            context.Configuration.GetSection(MyApiConfiguration.Configuration));

        // db
        services.AddDbContext<BotDbContext>(o =>
            o.UseNpgsql(context.Configuration.GetSection("Database:ConnectionString").Value));

        // services
        var types = typeof(ICommand).Assembly.GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type) && type.IsClass);
        foreach (var type in types)
        {
            services.AddScoped(typeof(ICommand), type);
        }
        
        services.AddSingleton<UpdateHandlers>();
        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var o = sp.GetService<IOptions<TelegramConfiguration>>()?.Value;
            TelegramBotClientOptions options = new(o!.Token);
            return new TelegramBotClient(options);
        });
        services.AddHostedService<PollingService>();
        services.AddHostedService<FetchFileTimedService>();

        services.AddHttpClient();
        services.AddScoped<IMyApiService, MyApiService>();
    })
    .Build();

await host.RunAsync();