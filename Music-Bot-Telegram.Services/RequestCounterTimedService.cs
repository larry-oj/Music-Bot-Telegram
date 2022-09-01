using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Music_Bot_Telegram.Data;
using Music_Bot_Telegram.Data.Models;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace Music_Bot_Telegram.Services;

public class RequestCounterTimedService : IHostedService, IDisposable
{
    private readonly ILogger<RequestCounterTimedService> _logger;
    private Timer? _timer = null;
    private readonly IServiceProvider _serviceProvider;

    public RequestCounterTimedService(
        ILogger<RequestCounterTimedService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, 
            TimeSpan.FromHours(2));

        return Task.CompletedTask;
    }

    private async void DoWorkAsync(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        RequestMeter record;
        if (!context.RequestMeters.Any())
        {
            record = new RequestMeter();
            context.Add(record);
        }
        else
        {
            record = context.RequestMeters.FirstOrDefault()!;
            if (DateTime.UtcNow.Day != record.CurrentDate.Day)
            {
                record.CurrentDate = DateTime.UtcNow;
                record.RequestCount = 0;
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