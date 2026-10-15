using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.API.DTOs
{
    public record CreateGenreRequest(
        [Required, MaxLength(100)]
        string Name
    );

    public record EditGenreRequest(
        [Required]
        int Id,
        [Required, MaxLength(100)]
        string Name
    );

    public record GenreResponse(
        int Id,
        string Name
    );
}
