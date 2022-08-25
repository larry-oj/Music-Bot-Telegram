using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace Music_Bot_Telegram.Services;

public class FetchFileTimedService : IHostedService, IDisposable
{
    private readonly ILogger<FetchFileTimedService> _logger;
    private Timer? _timer = null;
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;

    public FetchFileTimedService(
        ILogger<FetchFileTimedService> logger,
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _botClient = botClient;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, 
            TimeSpan.FromSeconds(10));

        return Task.CompletedTask;
    }

    private async void DoWorkAsync(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        var api = scope.ServiceProvider.GetRequiredService<IMyApiService>();
        
        var users = context.Users.Where(u => u.IsActiveConversion);
        foreach (var user in users)
        {
            var response = await api.GetConversionStatusAsync(user.ConversionId!);
            if (response.IsFailed)
            {
                user.IsActiveConversion = false;
                user.ConversionId = null;
                await _botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: "Conversion failed!\nPlease try again later");
            }

            if (response.IsFinished)
            {
                var (fileName, audioStream) = await api.GetConversionResultAsync(user.ConversionId!);
                var audio = new InputOnlineFile(audioStream, fileName);
                await _botClient.SendAudioAsync(
                    chatId: user.Id,
                    audio: audio,
                    caption: "Conversion finished!");
                user.IsActiveConversion = false;
                user.ConversionId = null;
            }
        }

        await context.SaveChangesAsync();
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}