using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Logic.DTOs.Subscriptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlans()
    {
        var plans = await _subscriptionService.GetPlansAsync();
        return Ok(plans);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrent()
    {
        var subscription = await _subscriptionService.GetCurrentAsync(GetUserId());
        return subscription == null ? NotFound() : Ok(subscription);
    }

    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> GetHistory()
    {
        var subscriptions = await _subscriptionService.GetHistoryAsync(GetUserId());
        return Ok(subscriptions);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Subscribe(SubscribeRequest request)
    {
        var subscription = await _subscriptionService.SubscribeAsync(GetUserId(), request);
        return Ok(subscription);
    }

    [HttpPost("cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel()
    {
        await _subscriptionService.CancelAsync(GetUserId());
        return Ok(new { message = "Підписку скасовано" });
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
