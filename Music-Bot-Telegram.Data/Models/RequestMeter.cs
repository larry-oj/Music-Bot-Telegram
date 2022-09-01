using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Bot_Telegram.Data.Models;

public class RequestMeter : IEntity
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }

    public int RequestCount { get; set; } = 0;
    
    public DateTime CurrentDate { get; set; } = DateTime.UtcNow;
}