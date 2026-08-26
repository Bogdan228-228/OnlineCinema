using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Repositories
{
    public interface IUserActivityRepository
    {
        Task<UserActivity> AddUserActivity(UserActivity userActivity);
        Task<bool> UpdateMetadata(Guid userActivityId, string metadata);
        Task<bool> DeleteUserActivity(Guid userActivityId);
        Task<bool> DeleteActivityAsync(Guid userId, string entityId, EntityType entityType, ActionType actionType);
        Task<bool> Exists(Guid userId, string entityId, EntityType entityType, ActionType actionType);
        Task<List<UserActivity>> GetUserActivitiesByUserId(Guid userId);
        Task<List<UserActivity>> GetUserActivitiesByEntity(string entityId, EntityType entityType);
        Task<List<UserActivity>> GetUserActivitiesByAction(Guid userId, ActionType actionType);
    }
}
