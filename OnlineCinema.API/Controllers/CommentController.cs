using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;
using System.Security.Claims;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IUserActivityService _userActivityService;

        public CommentController(ICommentService commentService, IUserActivityService userActivityService)
        {
            _commentService = commentService;
            _userActivityService = userActivityService;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userId);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest request)
        {
            var comment = new Comment
            {
                UserId = GetCurrentUserId(),
                MovieId = request.MovieId,
                Content = request.Content
            };

            var created = await _commentService.AddCommentAsync(comment);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), created.Id.ToString(), EntityType.Comment, ActionType.Post, weight: 4.0);

            var response = new CommentResponse(
                created.Id,
                created.Content,
                created.CreatedAt,
                created.User,
                created.Movie
            );

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        [Authorize]
        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCommentRequest request)
        {
            var existing = await _commentService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (existing.UserId != GetCurrentUserId())
                return Forbid();

            existing.Content = request.Content;
            existing.MovieId = request.MovieId;

            await _commentService.UpdateComment(existing);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), existing.Id.ToString(), EntityType.Comment, ActionType.Put, weight: -4.0);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _commentService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (existing.UserId != GetCurrentUserId())
                return Forbid();

            await _commentService.DeleteCommentAsync(id);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), existing.Id.ToString(), EntityType.Comment, ActionType.Delete);

            return NoContent();
        }

        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null) return NotFound();

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), comment.Id.ToString(), EntityType.Comment, ActionType.View);

            var response = new CommentResponse(
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                comment.User,
                comment.Movie
            );

            return Ok(response);
        }

        [Authorize]
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            if (userId != currentUserId) return Forbid();

            var comments = await _commentService.GetByUserIdAsync(userId);

            await _userActivityService.AddActivityAsync(currentUserId, "", EntityType.Comment, ActionType.Search);

            var response = comments.Select(c => new CommentResponse(
                c.Id,
                c.Content,
                c.CreatedAt,
                c.User,
                c.Movie
            ));

            return Ok(response);
        }

        [HttpGet("movie/{movieId:guid}")]
        public async Task<IActionResult> GetByMovieId(Guid movieId)
        {
            var comments = await _commentService.GetByMovieIdAsync(movieId);

            await _userActivityService.AddActivityAsync(GetCurrentUserId(), "", EntityType.Comment, ActionType.Search);

            var response = comments.Select(c => new CommentResponse(
                c.Id,
                c.Content,
                c.CreatedAt,
                c.User,
                c.Movie
            ));

            return Ok(response);
        }
    }
}
