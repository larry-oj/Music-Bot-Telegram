using Music_Bot_Telegram.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Music_Bot_Telegram.Data.Models.User;

namespace Music_Bot_Telegram.Services.Commands;

public class Start : ICommand
{
    public string Name 
        => "start";
    
    public string Description 
        => "Bot introduction";

    public bool IsAdmin 
        => false;
    
    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, User user)
    {
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Greetings!\n I am a musical bird, that is capable of:\n 1 - Recognizing music from audio or video you send me!\n 2 - Create and send you .mp3 files of your favourite songs!\n 3 - Search music across YouTube and Spotify!\n \n Use /help to see the list of all commands!\n \n Creator - https://larryoj.tk\n Project GitHub - https://github.com/larry-oj/Music-Bot-Telegram");
    }
}