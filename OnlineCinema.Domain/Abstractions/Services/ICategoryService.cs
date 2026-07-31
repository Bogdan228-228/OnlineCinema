using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface ICategoryService
    {
        Task<Category> AddCategoryAsync(string name);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<Category?> EditCategoryAsync(int categoryId, string name);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoriesByNameAsync(string name);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
    }
}
