using System.Text.Json.Serialization;

namespace Music_Bot_Telegram.Services.Models;

public class SpotifySearchResponse
{
    [JsonPropertyName("tracks")]
    public List<Track> Tracks { get; set; }
}

public class Track
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("album")]
    public Album Album { get; set; }
    
    [JsonPropertyName("artists")]
    public List<Artist>? Artists { get; set; }
    
    [JsonPropertyName("duration_ms")]
    public int DurationMs { get; set; }
    
    [JsonPropertyName("explicit")]
    public bool Explicit { get; set; }
    
    [JsonPropertyName("external_urls")]
    public ExternalUrls ExternalUrls { get; set; }
    
    [JsonPropertyName("preview_url")]
    public string? PreviewUrl { get; set; }
}

public class Artist
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("images")]
    public List<Image>? Images { get; set; }
    
    [JsonPropertyName("external_urls")]
    public ExternalUrls ExternalUrls { get; set; }
}

public class Album
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("covers")]
    public List<Image> Covers { get; set; }
    
    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; }
    
    [JsonPropertyName("external_urls")]
    public ExternalUrls ExternalUrls { get; set; }
}

public class ExternalUrls
{
    [JsonPropertyName("spotify")]
    public string Spotify { get; set; }
}

public class Image
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
}