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
            .AnyAsync(f => f.UserId == userId && f.MovieId == Guid.Parse(request.ContentId));

        if (alreadyExists)
        {
            throw new ConflictException("Контент вже додано в обране");
        }

        var favorite = new Favorite
        {
            UserId = userId,
            MovieId = Guid.Parse(request.ContentId)
        };

        _dbContext.Favorites.Add(favorite);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(favorite);
    }

    public async Task<FavoriteResponse> RemoveAsync(Guid userId, Guid contentId)
    {
        var favorite = await _dbContext.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.MovieId == contentId);

        if (favorite == null)
        {
            throw new NotFoundException("Запис в обраному не знайдено");
        }

        _dbContext.Favorites.Remove(favorite);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(favorite);
    }

    public async Task<IReadOnlyList<FavoriteResponse>> GetAllAsync(Guid userId)
    {
        var favorites = await _dbContext.Favorites
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return favorites.Select(MapToResponse).ToList();
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, Guid contentId)
    {
        return await _dbContext.Favorites
            .AnyAsync(f => f.UserId == userId && f.MovieId == contentId);
    }

    private static FavoriteResponse MapToResponse(Favorite favorite)
    {
        return new FavoriteResponse
        {
            Id = favorite.Id,
            ContentId = favorite.MovieId,
            CreatedAt = favorite.CreatedAt
        };
    }
}
