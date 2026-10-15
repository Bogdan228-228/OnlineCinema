using System.ComponentModel.DataAnnotations;
using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Logic.DTOs.Reviews;

public class ModerateReviewRequest
{
    [Required]
    public ReviewStatus Status { get; set; }
}
