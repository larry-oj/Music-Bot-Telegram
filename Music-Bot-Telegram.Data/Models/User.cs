﻿using System.ComponentModel.DataAnnotations;

namespace Music_Bot_Telegram.Data.Models;

public class User : IEntity
{
    [Key] public long Id { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsBanned { get; set; } = false;
    public bool IsActiveSession { get; set; } = false;
    public bool IsActiveConversion { get; set; } = false;
    public string? ConversionId { get; set; }
    public List<Action> Actions { get; set; }

    public User()
    {
    }
    
    public User(long id)
    {
        Id = id;
    }
}