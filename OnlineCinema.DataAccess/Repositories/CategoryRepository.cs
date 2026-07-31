using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly OnlineCinemaDbContext _db;

        public CategoryRepository(OnlineCinemaDbContext db)
        {
            _db = db;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<Category> EditCategoryAsync(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _db.Categories.FindAsync(categoryId);
            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _db.Categories.FindAsync(categoryId);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoriesByNameAsync(string name)
        {
            return await _db.Categories.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
