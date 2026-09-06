using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IFavoriteService
    {
        Task<Favorite?> AddToFavoritesAsync(Guid userId, Guid movieId);
        Task<List<Favorite>> GetFavoritesByUserIdAsync(Guid userId);
        Task<Favorite?> RemoveFromFavoritesAsync(Guid favoriteId);
    }
}
