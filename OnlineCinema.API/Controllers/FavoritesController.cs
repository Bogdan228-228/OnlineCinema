using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Logic.DTOs.Favorites;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Authorize]
[Route("api/favorites")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var favorites = await _favoriteService.GetAllAsync(GetUserId());
        return Ok(favorites);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddFavoriteRequest request)
    {
        var favorite = await _favoriteService.AddAsync(GetUserId(), request);
        return Ok(favorite);
    }

    [HttpDelete("{contentId}")]
    public async Task<IActionResult> Remove(string contentId)
    {
        await _favoriteService.RemoveAsync(GetUserId(), contentId);
        return Ok(new { message = "Видалено з обраного" });
    }

    [HttpGet("{contentId}/status")]
    public async Task<IActionResult> IsFavorite(string contentId)
    {
        var isFavorite = await _favoriteService.IsFavoriteAsync(GetUserId(), contentId);
        return Ok(new { isFavorite });
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
