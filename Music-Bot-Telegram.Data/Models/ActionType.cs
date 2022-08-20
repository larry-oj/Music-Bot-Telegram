using System.ComponentModel.DataAnnotations;

namespace Music_Bot_Telegram.Data.Models;

public class ActionType : IEntity
{
    [Key] public int Id { get; set; }
    /* unique */ public string Name { get; set; }
    
    public ActionType()
    {
    }
    
    public ActionType(string name)
    {
        Name = name;
    }
}