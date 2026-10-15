using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface ICommentService
    {
        Task<Comment> AddCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Guid id);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<ICollection<Comment>> GetByMovieIdAsync(Guid movieId);
        Task<ICollection<Comment>> GetByUserIdAsync(Guid userId);
        Task<Comment> UpdateComment(Comment comment);
    }
}
