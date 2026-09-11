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

        public async Task<Movie> AddMovieAsync(Movie movie,
            List<int>? genreIds,
            List<Guid>? actorIds,
            List<int>? audioTrackIds,
            List<int>? platformIds)
        {
            if (genreIds != null)
                movie.Genres = await _db.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();

            if (actorIds != null)
                movie.Actors = await _db.Actors.Where(a => actorIds.Contains(a.Id)).ToListAsync();

            if (audioTrackIds != null)
                movie.AudioTracks = await _db.AudioTracks.Where(at => audioTrackIds.Contains(at.Id)).ToListAsync();

            if (platformIds != null)
                movie.Platforms = await _db.Platforms.Where(p => platformIds.Contains(p.Id)).ToListAsync();

            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();

            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .FirstAsync(m => m.Id == movie.Id);
        }

        public async Task<Movie> EditMovieAsync(Movie movie,
            List<int>? genreIds,
            List<Guid>? actorIds,
            List<int>? audioTrackIds,
            List<int>? platformIds)
        {
            if (genreIds != null)
                movie.Genres = await _db.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();

            if (actorIds != null)
                movie.Actors = await _db.Actors.Where(a => actorIds.Contains(a.Id)).ToListAsync();

            if (audioTrackIds != null)
                movie.AudioTracks = await _db.AudioTracks.Where(at => audioTrackIds.Contains(at.Id)).ToListAsync();

            if (platformIds != null)
                movie.Platforms = await _db.Platforms.Where(p => platformIds.Contains(p.Id)).ToListAsync();

            _db.Movies.Update(movie);
            await _db.SaveChangesAsync();

            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .FirstAsync(m => m.Id == movie.Id);
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
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .FirstOrDefaultAsync(m => m.Id == movieId);
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieByTitleAsync(string title)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .FirstOrDefaultAsync(m => m.Title.Contains(title));
        }

        public async Task<List<Movie>> GetMoviesByActorAsync(Guid actorId)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .Where(m => m.Actors.Any(a => a.Id == actorId))
                .ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByCategoryAsync(int categoryId)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .Where(m => m.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByGenreIdAsync(int genreId)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .Where(m => m.Genres.Any(g => g.Id == genreId))
                .ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByRatingAsync(decimal minRating)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .Where(m => m.Review >= minRating)
                .ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByYearAsync(int year)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .Where(m => m.DateRealise.Year == year)
                .ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByAudioTrackAsync(string language)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .Where(m => m.AudioTracks.Any(at => at.Language.Contains(language)))
                .ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByCountryAsync(string country)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .Where(m => m.Country.Contains(country))
                .ToListAsync();
        }

        public async Task<List<Movie>> GetMoviesByActorIdAsync(Guid actorId)
        {
            return await _db.Movies
                .Include(m => m.Category)
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.AudioTracks)
                .Include(m => m.Platforms)
                .Where(m => m.Actors.Any(a => a.Id == actorId))
                .ToListAsync();
        }
    }
}
