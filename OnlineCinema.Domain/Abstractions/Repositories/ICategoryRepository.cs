using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> EditCategoryAsync(Category category);
        Task<Category> DeleteCategoryAsync(int categoryId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoriesByNameAsync(string name);
    }
}
