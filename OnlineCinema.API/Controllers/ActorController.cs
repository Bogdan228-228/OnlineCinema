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
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;
        private readonly IUserActivityService _userActivityService;

        public ActorController(IActorService actorService, IUserActivityService userActivityService)
        {
            _actorService = actorService;
            _userActivityService = userActivityService;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddActor(CreateActorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var actor = await _actorService.AddActorAsync(request.FullName, request.Biography);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), actor.Id.ToString(), EntityType.Actor, ActionType.Post);

            var response = new ActorResponse(
                actor.Id,
                actor.FullName,
                actor.Biography
            );

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditActor(Guid id, EditActorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var actor = await _actorService.EditActorAsync(id, request.FullName, request.Biography);
            if (actor == null)
                return NotFound(new { message = "Actor not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), actor.Id.ToString(), EntityType.Actor, ActionType.Put);

            var response = new ActorResponse(
                actor.Id,
                actor.FullName,
                actor.Biography
            );

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteActor(Guid id)
        {
            var result = await _actorService.DeleteActorAsync(id);
            if (!result)
                return NotFound(new { message = "Actor not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), id.ToString(), EntityType.Actor, ActionType.Delete);

            return Ok(new { message = "Actor deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActorById(Guid id)
        {
            var actor = await _actorService.GetActorByIdAsync(id);
            if (actor == null)
                return NotFound(new { message = "Actor not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), actor.Id.ToString(), EntityType.Actor, ActionType.View);

            var response = new ActorResponse(
                actor.Id,
                actor.FullName,
                actor.Biography
            );

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllActors()
        {
            var actors = await _actorService.GetAllActorsAsync();

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Actor, ActionType.Search);

            var response = actors.Select(a => new ActorResponse(
                a.Id,
                a.FullName,
                a.Biography
            ));

            return Ok(response);
        }

        [HttpGet("get-by-fullname")]
        public async Task<IActionResult> GetActorsByFullName(string fullName)
        {
            var actors = await _actorService.GetActorsByFullNameAsync(fullName);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Actor, ActionType.Search);

            var response = actors.Select(a => new ActorResponse(
                a.Id,
                a.FullName,
                a.Biography
            ));

            return Ok(response);
        }
    }
}
