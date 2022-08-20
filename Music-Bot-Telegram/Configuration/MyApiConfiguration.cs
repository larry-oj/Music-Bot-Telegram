namespace Music_Bot_Telegram.Configuration;

public class MyApiConfiguration
{
    public static string Configuration => "MyApi";
    public string Url { get; set; } = "";
    public string RecognizeUrl { get; set; } = "";
    public string SpotifyUrl { get; set; } = "";
    public string YouTubeUrl { get; set; } = "";
    public string FileRequestUrl { get; set; } = "";
    public string FileStatusUrl { get; set; } = "";
    public string FileResultUrl { get; set; } = "";
}