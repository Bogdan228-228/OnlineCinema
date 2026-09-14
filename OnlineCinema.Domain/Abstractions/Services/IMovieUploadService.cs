using Microsoft.AspNetCore.Http;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IMovieUploadService
    {
        Task<Movie> UploadAndSliceAsync(Guid movieId, IFormFile file);
    }
}
