using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> AddCategoryAsync(string name)
        {
            var category = new Category
            {
                Name = name
            };
            return await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<Category?> EditCategoryAsync(int categoryId, string name)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return null;
            }
            category.Name = name;
            return await _categoryRepository.EditCategoryAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return false;
            }
            return await _categoryRepository.DeleteCategoryAsync(category.Id);
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _categoryRepository.GetCategoryByIdAsync(categoryId);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }

        public async Task<Category?> GetCategoriesByNameAsync(string name)
        {
            return await _categoryRepository.GetCategoriesByNameAsync(name);
        }
    }
}
