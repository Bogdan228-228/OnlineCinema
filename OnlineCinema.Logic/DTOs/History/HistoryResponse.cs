using System;

namespace OnlineCinema.Logic.DTOs.History;

public class HistoryResponse
{
    public Guid Id { get; set; }
    public string ContentId { get; set; } = null!;
    public int ProgressSeconds { get; set; }
    public int? DurationSeconds { get; set; }
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
