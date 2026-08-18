using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IUserActivityRepository
    {
        Task<UserActivity> AddUserActivity(UserActivity userActivity);
        Task<bool> DeleteActivityAsync(Guid userId, Guid movieId, ActionType actionType);
        Task<bool> Exists(Guid userId, Guid movieId, ActionType actionType);
        Task<List<UserActivity>> GetUserActivitiesByAction(Guid userId, ActionType actionType);
        Task<List<UserActivity>> GetUserActivitiesByMovieId(Guid movieId);
        Task<List<UserActivity>> GetUserActivitiesByUserId(Guid userId);
        Task<bool> UpdateMetadata(Guid userActivityId, string metadata);
    }
}
