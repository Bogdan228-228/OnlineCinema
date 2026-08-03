using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> EditCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByNameAsync(string name);
    }
}
