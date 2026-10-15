using Microsoft.AspNetCore.Http;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IActorUploadService
    {
        Task<Actor> UploadActorImageAsync(Guid actorId, IFormFile file);
        Task DeleteActorFilesAsync(Guid actorId, CancellationToken ct = default);
    }
}
