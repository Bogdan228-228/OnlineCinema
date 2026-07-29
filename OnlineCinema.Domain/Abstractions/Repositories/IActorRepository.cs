using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IActorRepository
    {
        Task<Actor> AddActorAsync(Actor actor);
        Task<Actor> EditActorAsync(Actor actor);
        Task<Actor> DeleteActorAsync(int actorId);
        Task<Actor?> GetActorByIdAsync(int actorId);
        Task<List<Actor>> GetAllActorsAsync();
        Task<List<Actor>> GetActorsByFullNameAsync(string name);
    }
}
