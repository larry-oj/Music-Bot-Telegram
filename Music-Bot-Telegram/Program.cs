using Music_Bot_Telegram.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<TelegramConfiguration>(
            context.Configuration.GetSection(TelegramConfiguration.Configuration));
        services.Configure<DatabaseConfiguration>(
            context.Configuration.GetSection(DatabaseConfiguration.Configuration));
    })
    .Build();

await host.RunAsync();