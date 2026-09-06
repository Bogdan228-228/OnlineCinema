using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.Logic.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUserActivityRepository _userActivityRepository;

        public UserActivityService(IUserActivityRepository userActivityRepository)
        {
            _userActivityRepository = userActivityRepository;
        }

        public async Task<UserActivity> AddActivityAsync(Guid userId, string? entityId, EntityType entityType, ActionType actionType, string? metadata = null, double weight = 1.0)
        {
            var activity = new UserActivity
            {
                UserId = userId,
                EntityId = entityId?.ToString(),
                EntityType = entityType,
                ActionType = actionType,
                Metadata = metadata,
                Timestamp = DateTime.UtcNow,
                Weight = weight
            };

            return await _userActivityRepository.AddUserActivity(activity);
        }

        public async Task<bool> UpdateMetadataAsync(Guid activityId, string metadata)
        {
            return await _userActivityRepository.UpdateMetadata(activityId, metadata);
        }

        public async Task<bool> DeleteActivityAsync(Guid userId, string entityId, EntityType entityType, ActionType actionType)
        {
            return await _userActivityRepository.DeleteActivityAsync(userId, entityId, entityType, actionType);
        }

        public async Task<bool> ExistsAsync(Guid userId, string entityId, EntityType entityType, ActionType actionType)
        {
            return await _userActivityRepository.Exists(userId, entityId, entityType, actionType);
        }

        public async Task<List<UserActivity>> GetActivitiesByUserAsync(Guid userId)
        {
            return await _userActivityRepository.GetUserActivitiesByUserId(userId);
        }

        public async Task<List<UserActivity>> GetActivitiesByEntityAsync(string entityId, EntityType entityType)
        {
            return await _userActivityRepository.GetUserActivitiesByEntity(entityId, entityType);
        }

        public async Task<List<UserActivity>> GetActivitiesByActionAsync(Guid userId, ActionType actionType)
        {
            return await _userActivityRepository.GetUserActivitiesByAction(userId, actionType);
        }
    }
}
