using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IGenreService
    {
        Task<Genre> AddGenreAsync(string name);
        Task<bool> DeleteGenreAsync(int genreId);
        Task<Genre?> EditGenreAsync(int genreId, string name);
        Task<List<Genre>> GetAllGenresAsync();
        Task<Genre?> GetGenreByIdAsync(int genreId);
        Task<Genre?> GetGenreByNameAsync(string name);
    }
}
