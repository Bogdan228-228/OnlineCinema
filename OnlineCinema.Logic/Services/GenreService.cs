using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<Genre> AddGenreAsync(string name)
        {
            var genre = new Genre
            {
                Name = name
            };
            return await _genreRepository.AddGenreAsync(genre);
        }

        public async Task<Genre?> EditGenreAsync(int genreId, string name)
        {
            var genre = await _genreRepository.GetGenreByIdAsync(genreId);
            if (genre != null)
            {
                genre.Name = name;
                return await _genreRepository.EditGenreAsync(genre);
            }
            return null;
        }

        public async Task<bool> DeleteGenreAsync(int genreId)
        {
            var genre = await _genreRepository.GetGenreByIdAsync(genreId);
            if (genre != null)
            {
                return await _genreRepository.DeleteGenreAsync(genreId);
            }
            return false;
        }

        public async Task<Genre?> GetGenreByIdAsync(int genreId)
        {
            return await _genreRepository.GetGenreByIdAsync(genreId);
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllGenresAsync();
        }

        public async Task<Genre?> GetGenreByNameAsync(string name)
        {
            return await _genreRepository.GetGenreByNameAsync(name);
        }
    }
}
