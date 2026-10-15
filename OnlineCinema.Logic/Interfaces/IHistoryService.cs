using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineCinema.Logic.DTOs.History;

namespace OnlineCinema.Logic.Interfaces;

public interface IHistoryService
{
    Task<HistoryResponse> UpsertProgressAsync(Guid userId, UpsertHistoryRequest request);
    Task<IReadOnlyList<HistoryResponse>> GetAllAsync(Guid userId);
    Task<HistoryResponse?> GetByContentAsync(Guid userId, string contentId);
    Task RemoveAsync(Guid userId, string contentId);
    Task ClearAllAsync(Guid userId);
}
