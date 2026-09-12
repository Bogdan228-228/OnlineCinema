using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineCinema.Logic.DTOs.Favorites;

namespace OnlineCinema.Logic.Interfaces;

public interface IFavoriteService
{
    Task<FavoriteResponse> AddAsync(Guid userId, AddFavoriteRequest request);
    Task RemoveAsync(Guid userId, string contentId);
    Task<IReadOnlyList<FavoriteResponse>> GetAllAsync(Guid userId);
    Task<bool> IsFavoriteAsync(Guid userId, string contentId);
}
