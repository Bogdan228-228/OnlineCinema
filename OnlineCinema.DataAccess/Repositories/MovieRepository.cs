using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly OnlineCinemaDbContext _db;

        public MovieRepository(OnlineCinemaDbContext db)
        {
            _db = db;
        }

        public async Task<Movie> AddMovieAsync(Movie movie)
        {
            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie> EditMovieAsync(Movie movie)
        {
            _db.Movies.Update(movie);
            await _db.SaveChangesAsync();
            return movie;
        }

        public async Task<bool> DeleteMovieAsync(Guid movieId)
        {
            var movie = await _db.Movies.FindAsync(movieId);
            if (movie != null)
            {
                _db.Movies.Remove(movie);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Movie?> GetMovieByIdAsync(Guid movieId)
        {
            return await _db.Movies.FindAsync(movieId);
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _db.Movies.ToListAsync();
        }

        public async Task<Movie?> GetMovieByTitleAsync(string title)
        {
            return await _db.Movies.FirstOrDefaultAsync(m => m.Title == title);
        }

        public async Task<List<Movie>> SearchMoviesByTitleAsync(string title)
        {
            return await _db.Movies.Where(m => m.Title.Contains(title)).ToListAsync();
        }

        public async Task<Movie?> GetMovieWithActorsAsync(Guid movieId)
        {
            return await _db.Movies.Include(m => m.Actors).FirstOrDefaultAsync(m => m.Id == movieId);
        }

        public async Task<List<Movie>> GetMoviesByCategoryAsync(int categoryId)
        {
            return await _db.Movies.Where(m => m.CategoryId == categoryId).ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByGenreAsync(string genreName)
        {
            return await _db.Movies.Where(m => m.Genres.Any(g => g.Name == genreName)).ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByRatingAsync(decimal minRating)
        {
            return await _db.Movies.Where(m => m.Review > minRating).ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByYearAsync(int year)
        {
            return await _db.Movies.Where(m => m.DateRealise.Year == year).ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByAudioTrackAsync(string language)
        {
            return await _db.Movies.Where(m => m.AudioTracks.Any(at => at.Language == language)).ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByCountryAsync(string country)
        {
            return await _db.Movies.Where(m => m.Country == country).ToListAsync();
        }
    }
}
