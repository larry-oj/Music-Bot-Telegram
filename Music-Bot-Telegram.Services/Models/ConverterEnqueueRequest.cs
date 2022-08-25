using System.Text.Json.Serialization;

namespace Music_Bot_Telegram.Services.Models;

public class ConverterEnqueueRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [JsonPropertyName("with_callback")] 
    public bool WithCallback { get; set; } = false;
    
    [JsonPropertyName("callback_url")] 
    public string? CallbackUrl { get; set; }

    public ConverterEnqueueRequest()
    {
        
    }
    
    public ConverterEnqueueRequest(string url)
    {
        Url = url;
    }

    public ConverterEnqueueRequest(string url, string callbackUrl)
    {
        Url = url;
        WithCallback = true;
        CallbackUrl = callbackUrl;
    }
}