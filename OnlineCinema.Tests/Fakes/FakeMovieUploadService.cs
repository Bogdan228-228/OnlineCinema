using Microsoft.AspNetCore.Http;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Tests.Fakes;

public class FakeMovieUploadService : IMovieUploadService
{
    public List<Guid> DeletedMovieIds { get; } = new();

    public Task DeleteMovieFilesAsync(Guid movieId, CancellationToken ct = default)
    {
        DeletedMovieIds.Add(movieId);
        return Task.CompletedTask;
    }

    public Task ProcessVideoInBackgroundAsync(Guid movieId, string tempPath) => Task.CompletedTask;
    public Task<Movie> UploadAndSliceVideoAsync(Guid movieId, IFormFile file) => throw new NotSupportedException();
    public Task<Movie> UploadPosterAsync(Guid movieId, IFormFile file) => throw new NotSupportedException();
    public Task<Movie> UploadTrailerAsync(Guid movieId, IFormFile file) => throw new NotSupportedException();
}