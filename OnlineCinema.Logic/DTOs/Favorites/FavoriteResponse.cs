using System;

namespace OnlineCinema.Logic.DTOs.Favorites;

public class FavoriteResponse
{
    public Guid Id { get; set; }
    public string ContentId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
