using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class ActorUploadService : IActorUploadService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IActorRepository _actorRepository;

        public ActorUploadService(BlobServiceClient blobServiceClient, IActorRepository actorRepository)
        {
            _blobServiceClient = blobServiceClient;
            _actorRepository = actorRepository;
        }

        public async Task<Actor> UploadActorImageAsync(Guid actorId, IFormFile file)
        {
            var actor = await _actorRepository.GetActorByIdAsync(actorId);
            if (actor == null)
            {
                throw new ArgumentException("Actor not found", nameof(actorId));
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient("public-assets");
            await containerClient.CreateIfNotExistsAsync();

            var blobName = $"avatars/actors/{actor.Id}/{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            actor.ImageUrl = blobClient.Uri.ToString();
            actor = await _actorRepository.EditActorAsync(actor);

            return actor;
        }

        public async Task DeleteActorFilesAsync(Guid actorId, CancellationToken ct = default)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("public-assets");

            await BlobCleaner.DeletePrefixAsync(containerClient, $"avatars/actors/{actorId}/", ct);
        }
    }
}
