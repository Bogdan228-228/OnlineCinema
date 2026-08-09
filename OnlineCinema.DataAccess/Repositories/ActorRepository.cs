using Microsoft.EntityFrameworkCore;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Repositories
{
    public class ActorRepository : IActorRepository
    {
        public readonly OnlineCinemaDbContext _db;

        public ActorRepository(OnlineCinemaDbContext db)
        {
            _db = db;
        }

        public async Task<Actor> AddActorAsync(Actor actor)
        {
            _db.Actors.Add(actor);
            await _db.SaveChangesAsync();
            return actor;
        }

        public async Task<Actor> EditActorAsync(Actor actor)
        {
            _db.Actors.Update(actor);
            await _db.SaveChangesAsync();
            return actor;
        }

        public async Task<bool> DeleteActorAsync(Guid actorId)
        {
            var actor = await _db.Actors.FindAsync(actorId);
            if (actor != null)
            {
                _db.Actors.Remove(actor);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Actor?> GetActorByIdAsync(Guid actorId)
        {
            return await _db.Actors.FindAsync(actorId);
        }

        public async Task<List<Actor>> GetAllActorsAsync()
        {
            return await _db.Actors.ToListAsync();
        }

        public async Task<List<Actor>> GetActorsByFullNameAsync(string name)
        {
            return await _db.Actors.Where(a => a.FullName.Contains(name)).ToListAsync();
        }
    }
}
