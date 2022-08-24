namespace Music_Bot_Telegram.Services.Configuration;

public class MyApiConfiguration
{
    public static string Configuration => "MyApi";
    public string Url { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public string RecognizeUrl { get; set; } = "";
    public string SearchUrl { get; set; } = "";
    public string ConverterUrl { get; set; } = "";
}