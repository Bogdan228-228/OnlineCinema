using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly OnlineCinemaDbContext _db;

        public GenreRepository(OnlineCinemaDbContext db)
        {
            _db = db;
        }

        public async Task<Genre> AddGenreAsync(Genre genre)
        {
            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();
            return genre;
        }

        public async Task<Genre> EditGenreAsync(Genre genre)
        {
            _db.Genres.Update(genre);
            await _db.SaveChangesAsync();
            return genre;
        }

        public async Task DeleteGenreAsync(int genreId)
        {
            var genre = await _db.Genres.FindAsync(genreId);
            if (genre != null)
            {
                _db.Genres.Remove(genre);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Genre?> GetGenreByIdAsync(int genreId)
        {
            return await _db.Genres.FindAsync(genreId);
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<Genre?> GetGenreByNameAsync(string name)
        {
            return await _db.Genres.FirstOrDefaultAsync(g => g.Name == name);
        }
    }
}
