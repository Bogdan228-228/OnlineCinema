using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly OnlineCinemaDbContext _db;

        public CommentRepository(OnlineCinemaDbContext db)
        {
            _db = db;
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            _db.Comments.Update(comment);
            await _db.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment != null)
            {
                _db.Comments.Remove(comment);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
            }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _db.Comments
                .Include(c => c.User)
                .Include(c => c.Movie)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ICollection<Comment>> GetByUserIdAsync(Guid userId)
        {
            return await _db.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.Movie)
                .ToListAsync();
        }

        public async Task<ICollection<Comment>> GetByMovieIdAsync(Guid movieId)
        {
            return await _db.Comments
                .Where(c => c.MovieId == movieId)
                .Include(c => c.User)
                .ToListAsync();
        }
    }
}
