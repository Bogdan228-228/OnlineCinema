using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AudioTrackController : ControllerBase
    {
        private readonly IAudioTrackService _audioTrackService;

        public AudioTrackController(IAudioTrackService audioTrackService)
        {
            _audioTrackService = audioTrackService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAudioTrack(CreateAudioTrackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var audioTrack = await _audioTrackService.AddAudioTrackAsync(request.Language);

            var response = new AudioTrackResponse(
                audioTrack.Id,
                audioTrack.Language
            );

            return Ok(response);
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditAudioTrack(int id, EditAudioTrackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var audioTrack = await _audioTrackService.EditAudioTrackAsync(id, request.Language);
            if (audioTrack == null)
                return NotFound(new { message = "Audio track not found" });

            var response = new AudioTrackResponse(
                audioTrack.Id,
                audioTrack.Language
            );

            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAudioTrack(int id)
        {
            var result = await _audioTrackService.DeleteAudioTrackAsync(id);
            if (!result)
                return NotFound(new { message = "Audio track not found" });

            return Ok(new { message = "Audio track deleted successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAudioTrackById(int id)
        {
            var audioTrack = await _audioTrackService.GetAudioTrackByIdAsync(id);
            if (audioTrack == null)
                return NotFound(new { message = "Audio track not found" });

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

            var response = new AudioTrackResponse(
                audioTrack.Id,
                audioTrack.Language
            );

            return Ok(response);
        }
    }
}
