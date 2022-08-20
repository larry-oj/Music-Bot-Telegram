using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Music_Bot_Telegram.Services;

public class UpdateHandlers
{
    private readonly ILogger<UpdateHandlers> _logger;
    private readonly IDbContextFactory<BotDbContext> _contextFactory;
    
    public UpdateHandlers(
        ILogger<UpdateHandlers> logger, 
        IDbContextFactory<BotDbContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }
    
    public Task PollingErrorHandler(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken = default)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _                                       => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update received: {0}", update);
        
        using var context = _contextFactory.CreateDbContext();
        
        // ...
    }

}