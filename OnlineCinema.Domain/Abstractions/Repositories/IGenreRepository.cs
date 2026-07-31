using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IGenreRepository
    {
        Task<Genre> AddGenreAsync(Genre genre);
        Task<Genre> EditGenreAsync(Genre genre);
        Task<bool> DeleteGenreAsync(int genreId);
        Task<Genre?> GetGenreByIdAsync(int genreId);
        Task<List<Genre>> GetAllGenresAsync();
        Task<Genre?> GetGenreByNameAsync(string name);
    }
}
