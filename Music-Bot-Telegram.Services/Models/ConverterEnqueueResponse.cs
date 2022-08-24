using System.Text.Json.Serialization;

namespace Music_Bot_Telegram.Services.Models;

public class ConverterEnqueueResponse
{
    [JsonPropertyName("id")] 
    public string Id { get; set; }
}