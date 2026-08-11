using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformService _platformService;

        public PlatformController(IPlatformService platformService)
        {
            _platformService = platformService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddPlatform(CreatePlatformRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var platform = await _platformService.AddPlatformAsync(request.Name);

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

            return Ok(new { message = "Platform deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlatformById(int id)
        {
            var platform = await _platformService.GetPlatformByIdAsync(id);
            if (platform == null)
                return NotFound(new { message = "Platform not found" });

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

            var response = new PlatformResponse(
                platform.Id,
                platform.Name
            );

            return Ok(response);
        }
    }
}
