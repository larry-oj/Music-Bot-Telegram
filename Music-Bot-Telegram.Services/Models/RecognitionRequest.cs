using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Music_Bot_Telegram.Services.Models;

public class RecognitionRequest
{
    [JsonPropertyName("url")]
    [Required]
    public string Url { get; set; }

    public RecognitionRequest(string url)
    {
        Url = url;
    }
}