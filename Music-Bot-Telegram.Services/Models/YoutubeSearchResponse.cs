using System.Text.Json.Serialization;

namespace Music_Bot_Telegram.Services.Models;

public class YouTubeSearchResponse
{
    [JsonPropertyName("items")]
    public List<Item> Items { get; set; }

    public override string ToString()
    {
        var @string = "";

        if (Items.Count == 0)
        {
            @string += "No videos found";
        }
        else
        {
            foreach (var item in Items)
            {
                @string += "Title: " + item.Snippet.Title + "\n";
                @string += "Channel: " + item.Snippet.ChannelTitle + "\n";
                @string += "Link: https://youtu.be/" + item.VideoId + "\n";
                @string += "\n";
            }
        }
        
        return @string;
    }
}

public class Item
{
    [JsonPropertyName("videoId")]
    public string VideoId { get; set; }

    [JsonPropertyName("snippet")]
    public Snippet Snippet { get; set; }
}

public class Snippet
{
    [JsonPropertyName("publishedAt")]
    public DateTime PublishedAt { get; set; }

    [JsonPropertyName("channelId")]
    public string ChannelId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("thumbnails")]
    public Thumbnails Thumbnails { get; set; }

    [JsonPropertyName("channelTitle")]
    public string ChannelTitle { get; set; }

    [JsonPropertyName("publishTime")]
    public DateTime PublishTime { get; set; }
}

public class Thumbnails
{
    [JsonPropertyName("default")]
    public Size Default { get; set; }

    [JsonPropertyName("medium")]
    public Size Medium { get; set; }

    [JsonPropertyName("high")]
    public Size High { get; set; }
}

public class Size
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}