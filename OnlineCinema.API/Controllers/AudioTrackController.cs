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
    public class AudioTrackController : ControllerBase
    {
        private readonly IAudioTrackService _audioTrackService;
        private readonly IUserActivityService _userActivityService;

        public AudioTrackController(IAudioTrackService audioTrackService, IUserActivityService userActivityService)
        {
            _audioTrackService = audioTrackService;
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
        public async Task<IActionResult> AddAudioTrack(CreateAudioTrackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var audioTrack = await _audioTrackService.AddAudioTrackAsync(request.Language);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), audioTrack.Id.ToString(), EntityType.AudioTrack, ActionType.Post);

            var response = new AudioTrackResponse(
                audioTrack.Id,
                audioTrack.Language
            );

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditAudioTrack(int id, EditAudioTrackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var audioTrack = await _audioTrackService.EditAudioTrackAsync(id, request.Language);
            if (audioTrack == null)
                return NotFound(new { message = "Audio track not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), audioTrack.Id.ToString(), EntityType.AudioTrack, ActionType.Put);

            var response = new AudioTrackResponse(
                audioTrack.Id,
                audioTrack.Language
            );

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAudioTrack(int id)
        {
            var result = await _audioTrackService.DeleteAudioTrackAsync(id);
            if (!result)
                return NotFound(new { message = "Audio track not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), id.ToString(), EntityType.AudioTrack, ActionType.Delete);

            return Ok(new { message = "Audio track deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAudioTrackById(int id)
        {
            var audioTrack = await _audioTrackService.GetAudioTrackByIdAsync(id);
            if (audioTrack == null)
                return NotFound(new { message = "Audio track not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), audioTrack.Id.ToString(), EntityType.AudioTrack, ActionType.View);

            var response = new AudioTrackResponse(
                audioTrack.Id,
                audioTrack.Language
            );

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAudioTracks()
        {
            var audioTracks = await _audioTrackService.GetAllAudioTracksAsync();

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.AudioTrack, ActionType.Search);

            var response = audioTracks.Select(at => new AudioTrackResponse(
                at.Id,
                at.Language
            ));

            return Ok(response);
        }

        [HttpGet("get-by-language")]
        public async Task<IActionResult> GetAudioTracksByLanguage(string language)
        {
            var audioTrack = await _audioTrackService.GetAudioTrackByLanguageAsync(language);
            if (audioTrack == null)
                return NotFound(new { message = "Audio track not found" });

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.AudioTrack, ActionType.Search);

            var response = new AudioTrackResponse(
                audioTrack.Id,
                audioTrack.Language
            );

            return Ok(response);
        }
    }
}
