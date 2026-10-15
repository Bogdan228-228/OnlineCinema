using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.Logic.DTOs.History;

public class UpsertHistoryRequest
{
    [Required]
    public string ContentId { get; set; } = null!;

    [Range(0, int.MaxValue)]
    public int ProgressSeconds { get; set; }

    [Range(1, int.MaxValue)]
    public int? DurationSeconds { get; set; }
}
