using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Logic.DTOs.History;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Authorize]
[Route("api/history")]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var history = await _historyService.GetAllAsync(GetUserId());
        return Ok(history);
    }

    [HttpGet("{contentId}")]
    public async Task<IActionResult> GetByContent(string contentId)
    {
        var history = await _historyService.GetByContentAsync(GetUserId(), contentId);
        return history == null ? NotFound() : Ok(history);
    }

    [HttpPut]
    public async Task<IActionResult> UpsertProgress(UpsertHistoryRequest request)
    {
        var history = await _historyService.UpsertProgressAsync(GetUserId(), request);
        return Ok(history);
    }

    [HttpDelete("{contentId}")]
    public async Task<IActionResult> Remove(string contentId)
    {
        await _historyService.RemoveAsync(GetUserId(), contentId);
        return Ok(new { message = "Запис видалено з історії" });
    }

    [HttpDelete]
    public async Task<IActionResult> ClearAll()
    {
        await _historyService.ClearAllAsync(GetUserId());
        return Ok(new { message = "Історію очищено" });
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
