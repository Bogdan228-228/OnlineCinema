using Microsoft.AspNetCore.SignalR;
using OnlineCinema.API.Hubs;
using OnlineCinema.Domain.Abstractions.Services;

namespace OnlineCinema.Logic.Services
{
    public class SignalRVideoProgressNotifier : IVideoProgressNotifier
    {
        private readonly IHubContext<MovieProgressHub> _hubContext;

        public SignalRVideoProgressNotifier(IHubContext<MovieProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyMovieReadyAsync(
            Guid movieId, string videoUrl, Dictionary<string, string> subtitleUrls)
        {
            return _hubContext.Clients
                .Group($"movie-{movieId}")
                .SendAsync("movieReady", new
                {
                    movieId,
                    videoUrl,
                    subtitleUrls
                });
        }

        public Task NotifyMovieFailedAsync(Guid movieId, string reason)
        {
            return _hubContext.Clients
                .Group($"movie-{movieId}")
                .SendAsync("movieFailed", new { movieId, reason });
        }
    }
}
