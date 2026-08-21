using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.API.DTOs;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;
using System.Security.Claims;

namespace OnlineCinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
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
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCommentRequest request)
        {
            var existing = await _commentService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (existing.UserId != GetCurrentUserId())
                return Forbid();

            existing.Content = request.Content;
            existing.MovieId = request.MovieId;

            await _commentService.UpdateComment(existing);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _commentService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (existing.UserId != GetCurrentUserId())
                return Forbid();

            await _commentService.DeleteCommentAsync(id);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null) return NotFound();

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
