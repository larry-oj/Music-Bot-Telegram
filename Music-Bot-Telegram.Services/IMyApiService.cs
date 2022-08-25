using Music_Bot_Telegram.Services.Models;

namespace Music_Bot_Telegram.Services;

public interface IMyApiService
{
    Task<object> RecognizeAsync(string url);
    Task<YouTubeSearchResponse> SearchYoutubeAsync(string query);
    Task<SpotifySearchResponse> SearchSpotifyAsync(string query);
    Task<ConverterEnqueueResponse> EnqueueConversionAsync(string url);
    Task<ConverterStatusResponse> GetConversionStatusAsync(string id);
    Task<(string, Stream)> GetConversionResultAsync(string id);
}