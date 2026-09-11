using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.Logic.DTOs.Reviews;

public class CreateReviewRequest
{
    [Required]
    public string ContentId { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Text { get; set; }
}
