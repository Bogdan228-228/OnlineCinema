using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Abstractions.Repositories;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;

public class UserActivityRepository : IUserActivityRepository
{
    private readonly OnlineCinemaDbContext _db;

    public UserActivityRepository(OnlineCinemaDbContext db)
    {
        _db = db;
    }

    public async Task<UserActivity> AddUserActivity(UserActivity userActivity)
    {
        userActivity.User = await _db.Users.FindAsync(userActivity.UserId);

        if (userActivity.User == null)
            throw new InvalidOperationException("Invalid user specified.");

        _db.UserActivities.Add(userActivity);
        await _db.SaveChangesAsync();

        return await _db.UserActivities
            .Include(ua => ua.User)
            .FirstAsync(ua => ua.Id == userActivity.Id);
    }

    public async Task<bool> UpdateMetadata(Guid userActivityId, string metadata)
    {
        var userActivity = await _db.UserActivities.FindAsync(userActivityId);
        if (userActivity != null)
        {
            userActivity.Metadata = metadata;
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteUserActivity(Guid userActivityId)
    {
        var userActivity = await _db.UserActivities.FindAsync(userActivityId);
        if (userActivity != null)
        {
            _db.UserActivities.Remove(userActivity);
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteActivityAsync(Guid userId, string entityId, EntityType entityType, ActionType actionType)
    {
        var userActivity = await _db.UserActivities
            .FirstOrDefaultAsync(ua => ua.UserId == userId
                                    && ua.EntityId == entityId.ToString()
                                    && ua.EntityType == entityType
                                    && ua.ActionType == actionType);

        if (userActivity == null)
            return false;

        _db.UserActivities.Remove(userActivity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Exists(Guid userId, string entityId, EntityType entityType, ActionType actionType)
    {
        return await _db.UserActivities
            .AnyAsync(ua => ua.UserId == userId
                         && ua.EntityId == entityId.ToString()
                         && ua.EntityType == entityType
                         && ua.ActionType == actionType);
    }

    public async Task<List<UserActivity>> GetUserActivitiesByUserId(Guid userId)
    {
        return await _db.UserActivities
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.User)
            .ToListAsync();
    }

    public async Task<List<UserActivity>> GetUserActivitiesByEntity(string entityId, EntityType entityType)
    {
        return await _db.UserActivities
            .Where(ua => ua.EntityId == entityId.ToString() && ua.EntityType == entityType)
            .Include(ua => ua.User)
            .ToListAsync();
    }

    public async Task<List<UserActivity>> GetUserActivitiesByAction(Guid userId, ActionType actionType)
    {
        return await _db.UserActivities
            .Where(ua => ua.UserId == userId && ua.ActionType == actionType)
            .Include(ua => ua.User)
            .ToListAsync();
    }

    public async Task<List<UserActivity>> GetMovieActivitiesAsync()
    {
        return await _db.UserActivities
            .Where(a => a.EntityType == EntityType.Movie)
            .ToListAsync();
    }
}
