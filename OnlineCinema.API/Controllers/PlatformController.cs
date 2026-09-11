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
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformService;
        private readonly IUserActivityService _userActivityService;

        public PlatformController(IPlatformService platformService, IUserActivityService userActivityService)
        {
            _platformService = platformService;
            _userActivityService = userActivityService;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddPlatform(CreatePlatformRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var platform = await _platformService.AddPlatformAsync(request.Name);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), platform.Id.ToString(), EntityType.Platform, ActionType.Post);

            var response = new PlatformResponse(
                platform.Id,
                platform.Name
            );

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditPlatform(int id, EditPlatformRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var platform = await _platformService.EditPlatformAsync(id, request.Name);
            if (platform == null)
                return NotFound(new { message = "Platform not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), platform.Id.ToString(), EntityType.Platform, ActionType.Put);

            var response = new PlatformResponse(
                platform.Id,
                platform.Name
            );

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePlatform(int id)
        {
            var result = await _platformService.DeletePlatformAsync(id);
            if (!result)
                return NotFound(new { message = "Platform not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), id.ToString(), EntityType.Platform, ActionType.Delete);

            return Ok(new { message = "Platform deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlatformById(int id)
        {
            var platform = await _platformService.GetPlatformByIdAsync(id);
            if (platform == null)
                return NotFound(new { message = "Platform not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), platform.Id.ToString(), EntityType.Platform, ActionType.View);

            var response = new PlatformResponse(
                platform.Id,
                platform.Name
            );

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPlatforms()
        {
            var platforms = await _platformService.GetAllPlatformsAsync();

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Platform, ActionType.Search);

            var response = platforms.Select(p => new PlatformResponse(
                p.Id,
                p.Name
            ));

            return Ok(response);
        }

        [HttpGet("get-by-name")]
        public async Task<IActionResult> GetPlatformByName(string name)
        {
            var platform = await _platformService.GetPlatformByNameAsync(name);
            if (platform == null)
                return NotFound(new { message = "Platform not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Platform, ActionType.Search);

            var response = new PlatformResponse(
                platform.Id,
                platform.Name
            );

            return Ok(response);
        }
    }
}
