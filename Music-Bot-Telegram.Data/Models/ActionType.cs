using System.ComponentModel.DataAnnotations;

namespace Music_Bot_Telegram.Data.Models;

public class ActionType : IEntity
{
    [Key] public long Id { get; set; }
    /* unique */ public string Name { get; set; }
    
    public ActionType()
    {
    }
    
    public ActionType(string name)
    {
        Name = name;
    }
}