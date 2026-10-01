using OnlineCinema.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.API.DTOs
{
    public record CreateCommentRequest(
        [Required]
        Guid MovieId,
        [Required]
        string Content
    );

    public record CommentResponse(
        Guid Id,
        string Content,
        DateTime CreatedAt,
        User User,
        Movie Movie
    );
}
