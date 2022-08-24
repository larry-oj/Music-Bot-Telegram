namespace Music_Bot_Telegram.Services.Exceptions;

public class ApiErrorResponse : Exception
{
    public ApiErrorResponse()
    {
    }
    
    public ApiErrorResponse(string message) 
        : base(message)
    {
    }
}