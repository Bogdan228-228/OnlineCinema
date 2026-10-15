using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.API.DTOs
{
    public record CreatePlatformRequest(
        [Required, MaxLength(100)]
        string Name
    );

    public record EditPlatformRequest(
        [Required]
        int Id,
        [Required, MaxLength(100)]
        string Name
    );

    public record PlatformResponse(
        int Id,
        string Name
    );
}
