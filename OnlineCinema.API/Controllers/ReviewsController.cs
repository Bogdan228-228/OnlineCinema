using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Domain.Constants;
using OnlineCinema.Logic.DTOs.Reviews;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("content/{contentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForContent(string contentId)
    {
        var reviews = await _reviewService.GetForContentAsync(contentId);
        return Ok(reviews);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMine()
    {
        var reviews = await _reviewService.GetMyReviewsAsync(GetUserId());
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateReviewRequest request)
    {
        var review = await _reviewService.CreateAsync(GetUserId(), request);
        return Ok(review);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, UpdateReviewRequest request)
    {
        var review = await _reviewService.UpdateAsync(GetUserId(), id, request);
        return Ok(review);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _reviewService.DeleteAsync(GetUserId(), id);
        return Ok(new { message = "Відгук видалено" });
    }

    [HttpGet("pending")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> GetPending()
    {
        var reviews = await _reviewService.GetPendingAsync();
        return Ok(reviews);
    }

    [HttpPatch("{id:guid}/moderate")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Moderate(Guid id, ModerateReviewRequest request)
    {
        var review = await _reviewService.ModerateAsync(id, request);
        return Ok(review);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
