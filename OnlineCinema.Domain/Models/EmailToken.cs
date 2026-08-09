using System;
using System.Collections.Generic;
using System.Text;
using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Domain.Models;

public class EmailToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;
    public TokenType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

