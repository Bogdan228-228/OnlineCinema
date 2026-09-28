using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> AddAsync(Comment comment);
        Task<bool> DeleteAsync(Guid id);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<ICollection<Comment>> GetByMovieIdAsync(Guid movieId);
        Task<ICollection<Comment>> GetByUserIdAsync(Guid userId);
        Task<Comment> UpdateAsync(Comment comment);
    }
}
