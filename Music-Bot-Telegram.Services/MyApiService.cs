using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Music_Bot_Telegram.Services.Configuration;
using Music_Bot_Telegram.Services.Exceptions;
using Music_Bot_Telegram.Services.Extensions;
using Music_Bot_Telegram.Services.Models;

namespace Music_Bot_Telegram.Services;

public class MyApiService : IMyApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyApiService> _logger;
    private readonly MyApiConfiguration _options;

    public MyApiService(
        IHttpClientFactory clientFactory, 
        ILogger<MyApiService> logger,
        IOptions<MyApiConfiguration> options)
    {
        _logger = logger;
        _options = options.Value;
        _httpClient = clientFactory.CreateClient();
    }


    public async Task<object> RecognizeAsync(string url)
    {
        throw new NotImplementedException();
    }

    public async Task<YouTubeSearchResponse> SearchYoutubeAsync(string query)
    {
        var uri = new Uri($"{_options.BaseUrl}/{_options.SearchUrl}/youtube")
            .AppendParameter("query", query)
            .AppendParameter("limit", "5");
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = uri
        };
        
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new ApiErrorResponse(content);
        
        return JsonSerializer.Deserialize<YouTubeSearchResponse>(content)!;
    }

    public async Task<SpotifySearchResponse> SearchSpotifyAsync(string query)
    {
        var uri = new Uri($"{_options.BaseUrl}/{_options.SearchUrl}/spotify")
            .AppendParameter("query", query)
            .AppendParameter("limit", "5");
        
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = uri
        };
        
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new ApiErrorResponse(content);
        
        return JsonSerializer.Deserialize<SpotifySearchResponse>(content)!;
    }

    public async Task<ConverterEnqueueResponse> EnqueueConversionAsync(string url)
    {
        var uri = new Uri($"{_options.BaseUrl}/{_options.ConverterUrl}/videos");

        var body = new StringContent("{\"url\":\"" + url + "\"}");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = uri,
            Content = body
        };
        
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new ApiErrorResponse(content);
        
        return JsonSerializer.Deserialize<ConverterEnqueueResponse>(content)!;
    }

    public async Task<ConverterStatusResponse> GetConversionStatusAsync(string id)
    {
        var uri = new Uri($"{_options.BaseUrl}/{_options.ConverterUrl}/vidoes/{id}/status");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = uri
        };
        
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) throw new ApiErrorResponse(content);
        
        return JsonSerializer.Deserialize<ConverterStatusResponse>(content)!;
    }

    public async Task<Stream> GetConversionResultAsync(string id)
    {
        var uri = new Uri($"{_options.BaseUrl}/{_options.ConverterUrl}/vidoes/{id}");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = uri
        };
        
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStreamAsync();
        if (!response.IsSuccessStatusCode) throw new ApiErrorResponse();
        
        return content!;
    }
}