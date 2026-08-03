using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddActor(CreateActorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var actor = await _actorService.AddActorAsync(request.FullName, request.Biography);

            var response = new ActorResponse(
                actor.Id,
                actor.FullName,
                actor.Biography
            );

            return Ok(response);
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditActor(Guid id, EditActorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var actor = await _actorService.EditActorAsync(id, request.FullName, request.Biography);
            if (actor == null)
                return NotFound(new { message = "Actor not found" });

            var response = new ActorResponse(
                actor.Id,
                actor.FullName,
                actor.Biography
            );

            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteActor(Guid id)
        {
            var result = await _actorService.DeleteActorAsync(id);
            if (!result)
                return NotFound(new { message = "Actor not found" });

            return Ok(new { message = "Actor deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActorById(Guid id)
        {
            var actor = await _actorService.GetActorByIdAsync(id);
            if (actor == null)
                return NotFound(new { message = "Actor not found" });

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

            var response = actors.Select(a => new ActorResponse(
                a.Id,
                a.FullName,
                a.Biography
            ));

            return Ok(response);
        }
    }
}
