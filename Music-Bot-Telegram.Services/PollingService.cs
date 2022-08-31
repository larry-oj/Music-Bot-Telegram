using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Music_Bot_Telegram.Services;

public class PollingService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly UpdateHandlers _updateHandlers;
    private readonly ILogger<PollingService> _logger;

    public PollingService(
        ITelegramBotClient botClient,
        UpdateHandlers updateHandlers,
        ILogger<PollingService> logger)
    {
        _botClient = botClient;
        _updateHandlers = updateHandlers;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true,
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var me = await _botClient.GetMeAsync(stoppingToken);
                _logger.LogInformation("Start receiving updates for {BotName}", me.Username ?? "Bot");

                // Start receiving updates
                await _botClient.ReceiveAsync(
                    updateHandler: _updateHandlers.HandleUpdateAsync,
                    pollingErrorHandler: _updateHandlers.PollingErrorHandler,
                    receiverOptions: receiverOptions,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError ("Polling failed with exception: {Exception}", ex);

                await _botClient.SendTextMessageAsync(425649959, "Bot has thrown an error:\n" + ex, cancellationToken: stoppingToken);

                // Cool down if something goes wrong
                await Task.Delay(2_0000, stoppingToken);
            }
        }
    }
}