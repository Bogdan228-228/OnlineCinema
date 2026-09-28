using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Domain.Abstractions.Services
{
    public interface IUserActivityService
    {
        Task<UserActivity> AddActivityAsync(Guid userId, string? entityId, EntityType entityType, ActionType actionType, string? metadata = null, double weight = 1.0);
        Task<bool> UpdateMetadataAsync(Guid activityId, string metadata);
        Task<bool> DeleteActivityAsync(Guid userId, string? entityId, EntityType entityType, ActionType actionType);
        Task<bool> ExistsAsync(Guid userId, string? entityId, EntityType entityType, ActionType actionType);
        Task<List<UserActivity>> GetActivitiesByUserAsync(Guid userId);
        Task<List<UserActivity>> GetActivitiesByEntityAsync(string? entityId, EntityType entityType);
        Task<List<UserActivity>> GetActivitiesByActionAsync(Guid userId, ActionType actionType);
    }
}
