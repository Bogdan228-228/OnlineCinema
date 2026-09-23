using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Models;
using System.Diagnostics;
using Whisper.net;
using Whisper.net.Ggml;

namespace OnlineCinema.Logic.Services
{
    public class MovieUploadService : IMovieUploadService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IMovieRepository _movieRepository;
        private readonly WhisperFactory _whisperFactory;
        private readonly ILogger<MovieUploadService> _logger;

        private static readonly string[] SubtitleLanguages = { "en", "uk" };

        public MovieUploadService(BlobServiceClient blobServiceClient, IMovieRepository movieRepository, WhisperFactory whisperFactory, ILogger<MovieUploadService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _movieRepository = movieRepository;
            _whisperFactory = whisperFactory;
            _logger = logger;
        }

        private static async Task RunFfmpegAsync(string arguments)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var stderrTask = process.StandardError.ReadToEndAsync();
            var stdoutTask = process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();
            var stderr = await stderrTask;
            var stdout = await stdoutTask;

            if (process.ExitCode != 0)
                throw new InvalidOperationException(
                    $"ffmpeg failed (exit {process.ExitCode}):\n{stderr}");
        }

        public async Task<Movie> UploadAndSliceVideoAsync(Guid movieId, IFormFile file)
        {
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null)
            {
                throw new ArgumentException("Movie not found", nameof(movieId));
            }

            var tempPath = Path.GetTempFileName();
            await using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var outputDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(outputDirectory);

            try
            {
                var playlistPath = Path.Combine(outputDirectory, "index.m3u8");
                var ffmpegArgs = $"-y -i \"{tempPath}\" -codec copy -start_number 0 -hls_time 10 -hls_list_size 0 -f hls \"{playlistPath}\"";

                _logger.LogInformation("Starting HLS slicing for movie {MovieId}", movie.Id);
                await RunFfmpegAsync(ffmpegArgs);

                var audioPath = Path.Combine(outputDirectory, "audio.wav");
                _logger.LogInformation("Extracting audio for movie {MovieId}", movie.Id);
                await RunFfmpegAsync($"-y -i \"{tempPath}\" -vn -ac 1 -ar 16000 -c:a pcm_s16le \"{audioPath}\"");

                var generatedSubtitles = new Dictionary<string, string>();
                foreach (var lang in SubtitleLanguages)
                {
                    var vttPath = Path.Combine(outputDirectory, $"subs_{lang}.vtt");
                    await GenerateSubtitlesAsync(audioPath, vttPath, lang);
                    generatedSubtitles[lang] = vttPath;
                }

                var duration = await GetVideoDurationAsync(tempPath);

                var containerClient = _blobServiceClient.GetBlobContainerClient("private-media");
                await containerClient.CreateIfNotExistsAsync();

                var hlsFiles = Directory.GetFiles(outputDirectory, "*.ts")
                    .Concat(Directory.GetFiles(outputDirectory, "*.m3u8"));

                foreach (var filePath in hlsFiles)
                {
                    var blobClient = containerClient.GetBlobClient(
                        $"videos/{movie.Id}/{Path.GetFileName(filePath)}");

                    var contentType = Path.GetExtension(filePath).ToLowerInvariant() switch
                    {
                        ".m3u8" => "application/vnd.apple.mpegurl",
                        ".ts" => "video/mp2t",
                        _ => "application/octet-stream"
                    };

                    await blobClient.UploadAsync(filePath, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
                    });
                }

                _logger.LogInformation("Uploaded HLS for movie {MovieId}", movie.Id);

                foreach (var (lang, vttPath) in generatedSubtitles)
                {
                    if (!File.Exists(vttPath)) continue;

                    var blobClient = containerClient.GetBlobClient(
                        $"subtitles/{movie.Id}/{lang}.vtt");

                    await blobClient.UploadAsync(vttPath, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = "text/vtt; charset=utf-8"
                        }
                    });

                    _logger.LogInformation(
                        "Uploaded subtitles {Lang} for movie {MovieId}", lang, movie.Id);
                }

                movie.VideoUrl = $"{containerClient.Uri}/videos/{movie.Id}/index.m3u8";
                movie.Duration = duration;
                movie = await _movieRepository.EditMovieAsync(movie, null, null, null, null);
                return movie;
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                if (Directory.Exists(outputDirectory))
                    Directory.Delete(outputDirectory, recursive: true);
            }
        }

        private async Task GenerateSubtitlesAsync(string audioPath, string vttPath, string language)
        {
            _logger.LogInformation("Generating subtitles ({Lang}) from {Audio}", language, audioPath);

            using var processor = _whisperFactory.CreateBuilder()
                .WithLanguage(language)
                .WithThreads(Math.Max(1, Environment.ProcessorCount / 2))
                .Build();

            await using var audioStream = File.OpenRead(audioPath);
            await using var writer = new StreamWriter(
                vttPath, append: false, encoding: new System.Text.UTF8Encoding(false));

            await writer.WriteLineAsync("WEBVTT");
            await writer.WriteLineAsync();

            await foreach (var segment in processor.ProcessAsync(audioStream))
            {
                var text = segment.Text?.Trim();
                if (string.IsNullOrWhiteSpace(text)) continue;

                await writer.WriteLineAsync(
                    $"{FormatVttTimestamp(segment.Start)} --> {FormatVttTimestamp(segment.End)}");
                await writer.WriteLineAsync(text);
                await writer.WriteLineAsync();
            }

            await writer.FlushAsync();
        }

        private static string FormatVttTimestamp(TimeSpan t) => $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}";

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

            await using (var stream = file.OpenReadStream())
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

            await using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            movie.TrailerUrl = blobClient.Uri.ToString();
            movie = await _movieRepository.EditMovieAsync(movie, null, null, null, null);

            return movie;
        }

        private static async Task<TimeSpan> GetVideoDurationAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Video file not found", filePath);

            var ffprobeArgs = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{filePath}\"";

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffprobe",
                    Arguments = ffprobeArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = (await stdoutTask).Trim();
            var error = await stderrTask;

            if (process.ExitCode != 0)
                throw new InvalidOperationException($"ffprobe error: {error}");

            if (double.TryParse(output,
                System.Globalization.CultureInfo.InvariantCulture, out var seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }

            return TimeSpan.Zero;
        }

        public async Task DeleteMovieFilesAsync(Guid movieId, CancellationToken ct = default)
        {
            var privateContainer = _blobServiceClient.GetBlobContainerClient("private-media");
            var publicContainer = _blobServiceClient.GetBlobContainerClient("public-assets");

            await BlobCleaner.DeletePrefixAsync(privateContainer, $"videos/{movieId}/", ct);
            await BlobCleaner.DeletePrefixAsync(privateContainer, $"subtitles/{movieId}/", ct);
            await BlobCleaner.DeletePrefixAsync(privateContainer, $"trailers/{movieId}/", ct);
            await BlobCleaner.DeletePrefixAsync(publicContainer, $"posters/{movieId}/", ct);

            _logger.LogInformation("Deleted Blob files for movie {MovieId}", movieId);
        }
    }
}
