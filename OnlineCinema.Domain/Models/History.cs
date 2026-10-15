using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCinema.Domain.Models;

public class History
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string ContentId { get; set; } = null!;

    public int ProgressSeconds { get; set; }
    public int? DurationSeconds { get; set; }
    public bool Completed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
