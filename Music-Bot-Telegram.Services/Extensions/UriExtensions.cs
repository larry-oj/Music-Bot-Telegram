using System.Web;

namespace Music_Bot_Telegram.Services.Extensions;

public static class UriExtensions
{
    public static Uri AppendParameter(this Uri uri, string parameterName, string parameterValue)
    {
        var builder = new UriBuilder(uri);
        var query = HttpUtility.ParseQueryString(builder.Query);
        query[parameterName] = parameterValue;
        builder.Query = query.ToString();
        return builder.Uri;
    }
}