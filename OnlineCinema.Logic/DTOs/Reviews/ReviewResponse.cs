using System;
using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Logic.DTOs.Reviews;

public class ReviewResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ContentId { get; set; } = null!;
    public int Rating { get; set; }
    public string? Text { get; set; }
    public ReviewStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
