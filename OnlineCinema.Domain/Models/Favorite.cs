using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCinema.Domain.Models;

public class Favorite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string ContentId { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}