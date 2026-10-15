using OnlineCinema.Logic.DTOs.Favorites;

namespace OnlineCinema.Logic.Interfaces;

public interface IFavoriteService
{
    Task<FavoriteResponse> AddAsync(Guid userId, AddFavoriteRequest request);
    Task<FavoriteResponse> RemoveAsync(Guid userId, Guid contentId);
    Task<IReadOnlyList<FavoriteResponse>> GetAllAsync(Guid userId);
    Task<bool> IsFavoriteAsync(Guid userId, Guid contentId);
}
