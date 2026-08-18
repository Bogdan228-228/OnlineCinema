using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IUserActivityService
    {
        Task<UserActivity> AddActivityAsync(Guid userId, Guid movieId, ActionType actionType, string? metadata = null);
        Task<bool> DeleteActivityAsync(Guid userId, Guid movieId, ActionType actionType);
        Task<bool> ExistsAsync(Guid userId, Guid movieId, ActionType actionType);
        Task<List<UserActivity>> GetActivitiesByActionAsync(Guid userId, ActionType actionType);
        Task<List<UserActivity>> GetActivitiesByMovieAsync(Guid movieId);
        Task<List<UserActivity>> GetActivitiesByUserAsync(Guid userId);
        Task<bool> UpdateMetadataAsync(Guid activityId, string metadata);
    }
}
