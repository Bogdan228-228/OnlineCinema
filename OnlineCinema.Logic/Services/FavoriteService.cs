using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<Favorite?> AddToFavoritesAsync(Guid userId, Guid movieId)
        {
            var exists = await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
            if (exists.Any(f => f.MovieId == movieId))
            {
                return exists.First(f => f.MovieId == movieId);
            }

            var favorite = new Favorite
            {
                UserId = userId,
                MovieId = movieId
            };

            return await _favoriteRepository.AddFavoriteAsync(favorite);
        }

        public async Task<Favorite?> RemoveFromFavoritesAsync(Guid favoriteId)
        {
            var favorite = await _favoriteRepository.GetFavoriteByIdAsync(favoriteId);
            if (favorite == null) return null;

            await _favoriteRepository.DeleteFavoriteAsync(favoriteId);
            return favorite;
        }

        public async Task<List<Favorite>> GetFavoritesByUserIdAsync(Guid userId)
        {
            return await _favoriteRepository.GetFavoritesByUserIdAsync(userId);
        }
    }
}
