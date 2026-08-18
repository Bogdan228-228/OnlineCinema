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

        public async Task<UserActivity> AddActivityAsync(Guid userId, Guid movieId, ActionType actionType, string? metadata = null)
        {
            var activity = new UserActivity
            {
                UserId = userId,
                MovieId = movieId,
                ActionType = actionType,
                Metadata = metadata,
                Timestamp = DateTime.UtcNow
            };

            return await _userActivityRepository.AddUserActivity(activity);
        }

        public async Task<bool> UpdateMetadataAsync(Guid activityId, string metadata)
        {
            return await _userActivityRepository.UpdateMetadata(activityId, metadata);
        }

        public async Task<bool> DeleteActivityAsync(Guid userId, Guid movieId, ActionType actionType)
        {
            return await _userActivityRepository.DeleteActivityAsync(userId, movieId, actionType);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid movieId, ActionType actionType)
        {
            return await _userActivityRepository.Exists(userId, movieId, actionType);
        }

        public async Task<List<UserActivity>> GetActivitiesByUserAsync(Guid userId)
        {
            return await _userActivityRepository.GetUserActivitiesByUserId(userId);
        }

        public async Task<List<UserActivity>> GetActivitiesByMovieAsync(Guid movieId)
        {
            return await _userActivityRepository.GetUserActivitiesByMovieId(movieId);
        }

        public async Task<List<UserActivity>> GetActivitiesByActionAsync(Guid userId, ActionType actionType)
        {
            return await _userActivityRepository.GetUserActivitiesByAction(userId, actionType);
        }
    }
}
