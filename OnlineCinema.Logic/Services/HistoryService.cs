using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.History;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class HistoryService : IHistoryService
{
    private const double CompletedThreshold = 0.9;

    private readonly OnlineCinemaDbContext _dbContext;

    public HistoryService(OnlineCinemaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HistoryResponse> UpsertProgressAsync(Guid userId, UpsertHistoryRequest request)
    {
        var history = await _dbContext.History
            .FirstOrDefaultAsync(h => h.UserId == userId && h.ContentId == request.ContentId);

        var completed = request.DurationSeconds.HasValue
            && request.DurationSeconds.Value > 0
            && request.ProgressSeconds >= request.DurationSeconds.Value * CompletedThreshold;

        if (history == null)
        {
            history = new History
            {
                UserId = userId,
                ContentId = request.ContentId,
                ProgressSeconds = request.ProgressSeconds,
                DurationSeconds = request.DurationSeconds,
                Completed = completed
            };

            _dbContext.History.Add(history);
        }
        else
        {
            history.ProgressSeconds = request.ProgressSeconds;
            history.DurationSeconds = request.DurationSeconds ?? history.DurationSeconds;
            history.Completed = completed || history.Completed;
            history.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        return MapToResponse(history);
    }

    public async Task<IReadOnlyList<HistoryResponse>> GetAllAsync(Guid userId)
    {
        var history = await _dbContext.History
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.UpdatedAt)
            .ToListAsync();

        return history.Select(MapToResponse).ToList();
    }

    public async Task<HistoryResponse?> GetByContentAsync(Guid userId, string contentId)
    {
        var history = await _dbContext.History
            .FirstOrDefaultAsync(h => h.UserId == userId && h.ContentId == contentId);

        return history == null ? null : MapToResponse(history);
    }

    public async Task RemoveAsync(Guid userId, string contentId)
    {
        var history = await _dbContext.History
            .FirstOrDefaultAsync(h => h.UserId == userId && h.ContentId == contentId);

        if (history != null)
        {
            _dbContext.History.Remove(history);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task ClearAllAsync(Guid userId)
    {
        var history = await _dbContext.History
            .Where(h => h.UserId == userId)
            .ToListAsync();

        _dbContext.History.RemoveRange(history);
        await _dbContext.SaveChangesAsync();
    }

    private static HistoryResponse MapToResponse(History history)
    {
        return new HistoryResponse
        {
            Id = history.Id,
            ContentId = history.ContentId,
            ProgressSeconds = history.ProgressSeconds,
            DurationSeconds = history.DurationSeconds,
            Completed = history.Completed,
            CreatedAt = history.CreatedAt,
            UpdatedAt = history.UpdatedAt
        };
    }
}
