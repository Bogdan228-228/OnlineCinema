using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;
using System.Diagnostics;

namespace OnlineCinema.Logic.Services
{
    public class MovieUploadService : IMovieUploadService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IMovieRepository _movieRepository;

        public MovieUploadService(BlobServiceClient blobServiceClient, IMovieRepository movieRepository)
        {
            _blobServiceClient = blobServiceClient;
            _movieRepository = movieRepository;
        }

        public async Task<Movie> UploadAndSliceVideoAsync(Guid movieId, IFormFile file)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null)
            {
                throw new ArgumentException("Movie not found", nameof(movieId));
            }

            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var outputDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(outputDirectory);

            var ffmpegArgs = $"-i \"{tempPath}\" -codec copy -start_number 0 -hls_time 10 -hls_list_size 0 -f hls \"{Path.Combine(outputDirectory, "index.m3u8")}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = ffmpegArgs,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            var containerClient = _blobServiceClient.GetBlobContainerClient("private-media");
            await containerClient.CreateIfNotExistsAsync();

            foreach (var filePath in Directory.GetFiles(outputDirectory))
            {
                var blobClient = containerClient.GetBlobClient($"videos/{movie.Id}/{Path.GetFileName(filePath)}");
                await blobClient.UploadAsync(filePath, overwrite: true);
            }

            File.Delete(tempPath);
            Directory.Delete(outputDirectory, recursive: true);

            movie.VideoUrl = $"{containerClient.Uri}/videos/{movie.Id}/index.m3u8";
            movie.Duration = await GetVideoDurationAsync(tempPath);
            movie = await _movieRepository.EditMovieAsync(movie, null, null, null, null);

            return movie;
        }

        public async Task<Movie> UploadPosterAsync(Guid movieId, IFormFile file)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null)
            {
                throw new ArgumentException("Movie not found", nameof(movieId));
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient("public-assets");
            await containerClient.CreateIfNotExistsAsync();

            var blobName = $"posters/{movie.Id}/{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            movie.PosterUrl = blobClient.Uri.ToString();
            movie = await _movieRepository.EditMovieAsync(movie, null, null, null, null);

            return movie;
        }

        public async Task<Movie> UploadTrailerAsync(Guid movieId, IFormFile file)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null)
            {
                throw new ArgumentException("Movie not found", nameof(movieId));
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient("private-media");
            await containerClient.CreateIfNotExistsAsync();

            var blobName = $"trailers/{movieId}/{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            movie.TrailerUrl = blobClient.Uri.ToString();
            movie = await _movieRepository.EditMovieAsync(movie, null, null, null, null);

            return movie;
        }

        private async Task<TimeSpan> GetVideoDurationAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Video file not found", filePath);

            var ffprobeArgs = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{filePath}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffprobe",
                    Arguments = ffprobeArgs,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = (await process.StandardOutput.ReadToEndAsync()).Trim();
            string error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new InvalidOperationException($"ffprobe error: {error}");

            if (double.TryParse(output, System.Globalization.CultureInfo.InvariantCulture, out var seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }

            return TimeSpan.Zero;
        }
    }
}
