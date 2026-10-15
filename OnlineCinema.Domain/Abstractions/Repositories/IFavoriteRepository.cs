using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IFavoriteRepository
    {
        Task<Favorite> AddFavoriteAsync(Favorite favorite);
        Task<bool> DeleteFavoriteAsync(Guid favoriteId);
        Task<Favorite?> GetFavoriteByIdAsync(Guid favoriteId);
        Task<List<Favorite>> GetFavoritesByUserIdAsync(Guid userId);
    }
}
