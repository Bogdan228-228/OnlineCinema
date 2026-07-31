using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _actorRepository;

        public ActorService(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<Actor?> AddActorAsync(string fullName, string biography)
        {
            var actor = new Actor
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Biography = biography
            };

            return await _actorRepository.AddActorAsync(actor);
        }

        public async Task<Actor?> EditActorAsync(Guid actorId, string fullName, string biography)
        {
            var actor = await _actorRepository.GetActorByIdAsync(actorId);
            if (actor == null)
            {
                return null;
            }
            actor.FullName = fullName;
            actor.Biography = biography;
            return await _actorRepository.EditActorAsync(actor);
        }

        public async Task<bool> DeleteActorAsync(Guid actorId)
        {
            var actor = await _actorRepository.GetActorByIdAsync(actorId);
            if (actor == null)
            {
                return false;
            }
            return await _actorRepository.DeleteActorAsync(actor.Id);
        }

        public async Task<Actor?> GetActorByIdAsync(Guid actorId)
        {
            return await _actorRepository.GetActorByIdAsync(actorId);
        }

        public async Task<List<Actor>> GetAllActorsAsync()
        {
            return await _actorRepository.GetAllActorsAsync();
        }

        public async Task<List<Actor>> GetActorsByFullNameAsync(string fullname)
        {
            return await _actorRepository.GetActorsByFullNameAsync(fullname);
        }
    }
}
