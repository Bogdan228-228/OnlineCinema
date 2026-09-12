using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;
using System.Security.Claims;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserActivityService _userActivityService;

        public CategoryController(ICategoryService categoryService, IUserActivityService userActivityService)
        {
            _categoryService = categoryService;
            _userActivityService = userActivityService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdString))
                throw new InvalidOperationException("User ID claim is missing");

            return Guid.Parse(userIdString);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddCategory(CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.AddCategoryAsync(request.Name);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), category.Id.ToString(), EntityType.Category, ActionType.Post);

            var response = new CategoryResponse(category.Id, category.Name);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditCategory(EditCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.EditCategoryAsync(request.Id, request.Name);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), category.Id.ToString(), EntityType.Category, ActionType.Put);

            var response = new CategoryResponse(category.Id, category.Name);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound(new { message = "Category not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), id.ToString(), EntityType.Category, ActionType.Delete);

            return Ok(new { message = "Category deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), category.Id.ToString(), EntityType.Category, ActionType.View);

            var response = new CategoryResponse(category.Id, category.Name);

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Category, ActionType.Search);

            var response = categories.Select(c => new CategoryResponse(c.Id, c.Name));

            return Ok(response);
        }

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var category = await _categoryService.GetCategoryByNameAsync(name);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Category, ActionType.Search);

            var response = new CategoryResponse(category.Id, category.Name);

            return Ok(response);
        }
    }
}
