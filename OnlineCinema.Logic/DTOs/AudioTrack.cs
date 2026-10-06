using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.API.DTOs
{
    public record CreateAudioTrackRequest(
        [Required, MaxLength(100)]
        string Language
    );

    public record EditAudioTrackRequest(
        [Required]
        int Id,
        [Required, MaxLength(100)]
        string Language
    );

    public record AudioTrackResponse(
        int Id,
        string Language
    );
}
