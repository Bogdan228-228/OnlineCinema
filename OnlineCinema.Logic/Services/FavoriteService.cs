using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Favorites;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class FavoriteService : IFavoriteService
{
    private readonly OnlineCinemaDbContext _dbContext;

    public FavoriteService(OnlineCinemaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FavoriteResponse> AddAsync(Guid userId, AddFavoriteRequest request)
    {
        var alreadyExists = await _dbContext.Favorites
            .AnyAsync(f => f.UserId == userId && f.ContentId == request.ContentId);

        if (alreadyExists)
        {
            throw new ConflictException("Контент вже додано в обране");
        }

        var favorite = new Favorite
        {
            UserId = userId,
            ContentId = request.ContentId
        };

        _dbContext.Favorites.Add(favorite);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(favorite);
    }

    public async Task RemoveAsync(Guid userId, string contentId)
    {
        var favorite = await _dbContext.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ContentId == contentId);

        if (favorite == null)
        {
            throw new NotFoundException("Запис в обраному не знайдено");
        }

        _dbContext.Favorites.Remove(favorite);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<FavoriteResponse>> GetAllAsync(Guid userId)
    {
        var favorites = await _dbContext.Favorites
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return favorites.Select(MapToResponse).ToList();
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, string contentId)
    {
        return await _dbContext.Favorites
            .AnyAsync(f => f.UserId == userId && f.ContentId == contentId);
    }

    private static FavoriteResponse MapToResponse(Favorite favorite)
    {
        return new FavoriteResponse
        {
            Id = favorite.Id,
            ContentId = favorite.ContentId,
            CreatedAt = favorite.CreatedAt
        };
    }
}
