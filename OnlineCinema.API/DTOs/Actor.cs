using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.API.DTOs
{
    public record CreateActorRequest(
        [Required, MaxLength(100)] 
        string FullName,
        [MaxLength(1000)] 
        string Biography
    );

    public record EditActorRequest(
        [Required] Guid Id,
        [Required, MaxLength(100)] 
        string FullName,
        [MaxLength(1000)] 
        string Biography
    );

    public record ActorResponse(
        Guid Id,
        string FullName,
        string Biography
    );
}
