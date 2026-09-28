using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly OnlineCinemaDbContext _db;

        public FavoriteRepository(OnlineCinemaDbContext db)
        {
            _db = db;
        }

        public async Task<Favorite> AddFavoriteAsync(Favorite favorite)
        {
            _db.Favorites.Add(favorite);
            await _db.SaveChangesAsync();
            return await _db.Favorites
                .Include(f => f.User)
                .Include(f => f.Movie)
                .FirstAsync(f => f.Id == favorite.Id);
        }

        public async Task<bool> DeleteFavoriteAsync(Guid favoriteId)
        {
            var favorite = await _db.Favorites.FindAsync(favoriteId);
            if (favorite == null)
            {
                return false;
            }
            _db.Favorites.Remove(favorite);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Favorite?> GetFavoriteByIdAsync(Guid favoriteId)
        {
            return await _db.Favorites
                .Include(f => f.User)
                .Include(f => f.Movie)
                .FirstOrDefaultAsync(f => f.Id == favoriteId);
        }

        public async Task<List<Favorite>> GetFavoritesByUserIdAsync(Guid userId)
        {
            return await _db.Favorites
                .Include(f => f.User)
                .Include(f => f.Movie)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }
    }
}
