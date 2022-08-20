using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Bot_Telegram.Data.Models;

public class Action : IEntity
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
    public User User { get; set; }
    public ActionType Type { get; set; }
    [Required] public string Data { get; set; }
    public int? Stage { get; set; }
    [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Action()
    {
    }
    
    public Action(User user, ActionType type, string data, int? stage)
    {
        User = user;
        Type = type;
        Data = data;
        Stage = stage;
    }
}