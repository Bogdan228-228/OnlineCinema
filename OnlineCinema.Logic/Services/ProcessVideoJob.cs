using Hangfire;
using Microsoft.Extensions.Logging;
using OnlineCinema.Domain.Abstractions.Services;

namespace OnlineCinema.Logic.Services
{
    public class ProcessVideoJob
    {
        private readonly IMovieUploadService _movieUploadService;
        private readonly ILogger<ProcessVideoJob> _logger;

        public ProcessVideoJob(IMovieUploadService movieUploadService, ILogger<ProcessVideoJob> logger)
        {
            _movieUploadService = movieUploadService;
            _logger = logger;
        }

        [Queue("video")]
        [DisableConcurrentExecution(timeoutInSeconds: 600)]
        public async Task RunAsync(Guid movieId, string tempPath)
        {
            _logger.LogInformation($"Starting video processing job for movieId: {movieId}");

            try
            {
                await _movieUploadService.ProcessVideoInBackgroundAsync(movieId, tempPath);
                _logger.LogInformation("Hangfire job completed for movie {MovieId}", movieId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hangfire job failed for movie {MovieId}", movieId);
                throw;
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }
    }
}
