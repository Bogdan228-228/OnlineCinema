using Microsoft.AspNetCore.Http;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IMovieUploadService
    {
        Task<Movie> UploadAndSliceVideoAsync(Guid movieId, IFormFile file);
        Task<Movie> UploadPosterAsync(Guid movieId, IFormFile file);
        Task<Movie> UploadTrailerAsync(Guid movieId, IFormFile file);
        Task DeleteMovieFilesAsync(Guid movieId, CancellationToken ct = default);
    }
}
