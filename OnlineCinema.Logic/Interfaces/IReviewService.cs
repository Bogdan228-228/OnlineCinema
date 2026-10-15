using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineCinema.Logic.DTOs.Reviews;

namespace OnlineCinema.Logic.Interfaces;

public interface IReviewService
{
    Task<ReviewResponse> CreateAsync(Guid userId, CreateReviewRequest request);
    Task<ReviewResponse> UpdateAsync(Guid userId, Guid reviewId, UpdateReviewRequest request);
    Task DeleteAsync(Guid userId, Guid reviewId);
    Task<IReadOnlyList<ReviewResponse>> GetForContentAsync(string contentId);
    Task<IReadOnlyList<ReviewResponse>> GetMyReviewsAsync(Guid userId);
    Task<IReadOnlyList<ReviewResponse>> GetPendingAsync();
    Task<ReviewResponse> ModerateAsync(Guid reviewId, ModerateReviewRequest request);
}
