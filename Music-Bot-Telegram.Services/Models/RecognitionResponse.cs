using System.Text.Json.Serialization;

namespace Music_Bot_Telegram.Services.Models;

public class RecognitionResponse
{
    [JsonPropertyName("artist")]
    public string Artist { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("album")]
    public string Album { get; set; }

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("timecode")]
    public string Timecode { get; set; }

    [JsonPropertyName("song_link")]
    public string SongLink { get; set; }
}

public class Error
{
    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; }
}