namespace Music_Bot_Telegram.Services.Exceptions;

public class OutOfRequests : Exception
{
    public OutOfRequests()
    {
    }
    
    public OutOfRequests(string message) 
        : base(message)
    {
    }
}