using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            return await _commentRepository.AddAsync(comment);
        }

        public async Task<Comment> UpdateComment(Comment comment)
        {
            return await _commentRepository.UpdateAsync(comment);
        }

        public async Task<bool> DeleteCommentAsync(Guid id)
        {
            return await _commentRepository.DeleteAsync(id);
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _commentRepository.GetByIdAsync(id);
        }

        public async Task<ICollection<Comment>> GetByUserIdAsync(Guid userId)
        {
            return await _commentRepository.GetByUserIdAsync(userId);
        }

        public async Task<ICollection<Comment>> GetByMovieIdAsync(Guid movieId)
        {
            return await _commentRepository.GetByMovieIdAsync(movieId);
        }
    }
}
