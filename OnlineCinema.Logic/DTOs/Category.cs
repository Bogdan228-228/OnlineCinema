using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.API.DTOs
{
    public record CreateCategoryRequest(
        [Required, MaxLength(100)]
        string Name
    );

    public record EditCategoryRequest(
        [Required]
        int Id,
        [Required, MaxLength(100)]
        string Name
    );

    public record CategoryResponse(
        int Id,
        string Name
    );
}
