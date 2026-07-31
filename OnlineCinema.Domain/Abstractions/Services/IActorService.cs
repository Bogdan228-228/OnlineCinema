using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IActorService
    {
        Task<Actor?> AddActorAsync(string fullName, string biography);
        Task<bool> DeleteActorAsync(Guid actorId);
        Task<Actor?> EditActorAsync(Guid actorId, string fullName, string biography);
        Task<Actor?> GetActorByIdAsync(Guid actorId);
        Task<List<Actor>> GetActorsByFullNameAsync(string fullname);
        Task<List<Actor>> GetAllActorsAsync();
    }
}
