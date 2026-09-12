using System;

namespace OnlineCinema.Logic.DTOs.Favorites;

public class FavoriteResponse
{
    public Guid Id { get; set; }
    public Guid ContentId { get; set; }
    public DateTime CreatedAt { get; set; }
}
