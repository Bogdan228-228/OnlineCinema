using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.Logic.DTOs.Favorites;

public class AddFavoriteRequest
{
    [Required]
    public string ContentId { get; set; } = null!;
}
