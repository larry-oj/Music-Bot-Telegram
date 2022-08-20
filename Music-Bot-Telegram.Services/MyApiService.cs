using Microsoft.Extensions.Options;

namespace Music_Bot_Telegram.Services;

public class MyApiService : IMyApiService
{
    private readonly IHttpClientFactory _clientFactory;

    public MyApiService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
}