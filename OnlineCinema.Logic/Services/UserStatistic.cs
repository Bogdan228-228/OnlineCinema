using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Logic.Dto;
using OnlineCinema.Logic.Interfaces;
using System.Text.Json;

namespace OnlineCinema.Logic.Services
{
    public class UserStatistic : IUserStatistic
    {
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);

        private readonly ILogger<UserStatistic> _logger;
        private readonly IUserService _userService;
        private readonly OnlineCinemaDbContext _context;
        private readonly IDistributedCache _cache;

        public UserStatistic(
            ILogger<UserStatistic> logger,
            IUserService userService,
            OnlineCinemaDbContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            _userService = userService;
            _context = context;
            _cache = cache;
        }

        private static string CacheKey(Guid userId) => $"user:stats:{userId}";

        public async Task<UserStatsDto> GetUserStatisticAsync(Guid userId)
        {
            var cacheKey = CacheKey(userId);

            var cached = await TryGetFromCacheAsync(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("Cache hit for user stats {UserId}", userId);
                return cached;
            }

            var user = await _userService.GetProfileAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                throw new ArgumentException("User not found", nameof(userId));
            }

            var stats = await ComputeStatsAsync(userId);
            await TrySetCacheAsync(cacheKey, stats);

            _logger.LogInformation("Computed stats for user {UserId}", userId);
            return stats;
        }

        public async Task InvalidateCacheAsync(Guid userId)
        {
            try
            {
                await _cache.RemoveAsync(CacheKey(userId));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to invalidate stats cache for {UserId}", userId);
            }
        }

        private async Task<UserStatsDto?> TryGetFromCacheAsync(string key)
        {
            try
            {
                var json = await _cache.GetStringAsync(key);
                return json is null ? null : JsonSerializer.Deserialize<UserStatsDto>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Stats cache read failed for {Key}", key);
                return null;
            }
        }

        private async Task TrySetCacheAsync(string key, UserStatsDto stats)
        {
            try
            {
                await _cache.SetStringAsync(
                    key,
                    JsonSerializer.Serialize(stats),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheTtl
                    });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Stats cache write failed for {Key}", key);
            }
        }

        private async Task<UserStatsDto> ComputeStatsAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);
            var monthAgo = now.AddDays(-30);

            var movieViews = _context.UserActivities
                .AsNoTracking()
                .Where(a => a.UserId == userId
                         && a.ActionType == ActionType.View
                         && a.EntityType == EntityType.Movie);

            var viewCounts = await movieViews
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Week = g.Count(a => a.Timestamp >= weekAgo),
                    Month = g.Count(a => a.Timestamp >= monthAgo)
                })
                .FirstOrDefaultAsync();

            var totalViews = viewCounts?.Total ?? 0;
            var viewsThisWeek = viewCounts?.Week ?? 0;
            var viewsThisMonth = viewCounts?.Month ?? 0;

            var reactions = await _context.UserActivities
                .AsNoTracking()
                .Where(a => a.UserId == userId
                         && (a.ActionType == ActionType.Like || a.ActionType == ActionType.Dislike))
                .GroupBy(a => a.ActionType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalLikes = reactions.FirstOrDefault(r => r.Type == ActionType.Like)?.Count ?? 0;
            var totalDislikes = reactions.FirstOrDefault(r => r.Type == ActionType.Dislike)?.Count ?? 0;

            var totalComments = await _context.Comments
                .AsNoTracking()
                .CountAsync(c => c.UserId == userId);

            var totalFavorites = await _context.Favorites
                .AsNoTracking()
                .CountAsync(f => f.UserId == userId);

            var lastViews = await movieViews
                .GroupBy(a => a.EntityId)
                .Select(g => new { EntityId = g.Key, LastViewed = g.Max(a => a.Timestamp) })
                .OrderByDescending(x => x.LastViewed)
                .ToListAsync();

            var viewedMovies = lastViews
                .Select(x => new { Id = Guid.TryParse(x.EntityId, out var id) ? id : Guid.Empty, x.LastViewed })
                .Where(x => x.Id != Guid.Empty)
                .ToList();

            var movieIds = viewedMovies.Select(x => x.Id).ToList();
            var recentIds = viewedMovies.Take(10).Select(x => x.Id).ToList();

            var recentMovies = await _context.Movies
                .AsNoTracking()
                .Where(m => recentIds.Contains(m.Id))
                .Select(m => new { m.Id, m.Title, m.PosterUrl })
                .ToDictionaryAsync(m => m.Id);

            var recentlyViewed = viewedMovies
                .Take(10)
                .Where(v => recentMovies.ContainsKey(v.Id))
                .Select(v =>
                {
                    var m = recentMovies[v.Id];
                    return new RecentViewDto(m.Id, m.Title, m.PosterUrl, v.LastViewed);
                })
                .ToList();

            var topGenres = (await _context.Movies
                    .AsNoTracking()
                    .Where(m => movieIds.Contains(m.Id))
                    .SelectMany(m => m.Genres)
                    .GroupBy(g => new { g.Id, g.Name })
                    .Select(g => new { g.Key.Name, Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .Take(5)
                    .ToListAsync())
                .Select(g => new TopGenreDto(g.Name, g.Count))
                .ToList();

            var topActors = (await _context.Movies
                    .AsNoTracking()
                    .Where(m => movieIds.Contains(m.Id))
                    .SelectMany(m => m.Actors)
                    .GroupBy(a => new { a.Id, a.FullName, a.ImageUrl })
                    .Select(g => new { g.Key.Id, g.Key.FullName, g.Key.ImageUrl, Count = g.Count() })
                    .OrderByDescending(a => a.Count)
                    .Take(5)
                    .ToListAsync())
                .Select(a => new TopActorDto(a.Id, a.FullName, a.ImageUrl, a.Count))
                .ToList();

            var days = await movieViews
                .Select(a => a.Timestamp.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToListAsync();

            var streak = CalculateStreak(days, now.Date);

            return new UserStatsDto(
                userId,
                totalViews,
                totalLikes,
                totalDislikes,
                totalComments,
                totalFavorites,
                viewsThisWeek,
                viewsThisMonth,
                streak,
                recentlyViewed,
                topGenres,
                topActors);
        }

        private static int CalculateStreak(List<DateTime> daysDesc, DateTime today)
        {
            if (daysDesc.Count == 0 || daysDesc[0] < today.AddDays(-1))
                return 0;

            var streak = 1;
            for (var i = 1; i < daysDesc.Count; i++)
            {
                if (daysDesc[i] != daysDesc[i - 1].AddDays(-1)) break;
                streak++;
            }
            return streak;
        }
    }
}