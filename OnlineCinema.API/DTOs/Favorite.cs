using OnlineCinema.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.API.DTOs
{
    public record CreateFavoriteRequest(
        [Required]
        Guid UserId,
        [Required]
        Guid MovieId
    );

    public record FavoriteResponse(
        Guid Id,
        User user,
        Movie movie
    );
}
