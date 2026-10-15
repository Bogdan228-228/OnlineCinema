using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Tests.Fakes;

public class FakeActorUploadService : IActorUploadService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public List<Guid> DeletedActorIds { get; } = new();

    public FakeActorUploadService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task<Actor> UploadActorImageAsync(Guid actorId, IFormFile file)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IActorRepository>();

        var actor = await repository.GetActorByIdAsync(actorId)
            ?? throw new ArgumentException("Actor not found", nameof(actorId));

        actor.ImageUrl = $"https://fake-storage.test/avatars/actors/{actor.Id}/{file.FileName}";
        return await repository.EditActorAsync(actor);
    }

    public Task DeleteActorFilesAsync(Guid actorId, CancellationToken ct = default)
    {
        DeletedActorIds.Add(actorId);
        return Task.CompletedTask;
    }
}