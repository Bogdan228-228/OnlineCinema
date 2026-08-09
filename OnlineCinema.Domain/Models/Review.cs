using System;
using System.Collections.Generic;
using System.Text;
using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Domain.Models;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string ContentId { get; set; } = null!;

    public int Rating { get; set; }
    public string? Text { get; set; }
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}