namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IVideoProgressNotifier
    {
        Task NotifyMovieReadyAsync(Guid movieId, string videoUrl, Dictionary<string, string> subtitleUrls);
        Task NotifyMovieFailedAsync(Guid movieId, string reason);
    }
}
