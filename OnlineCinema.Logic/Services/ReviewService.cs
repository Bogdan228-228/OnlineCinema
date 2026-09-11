using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Notifications;
using OnlineCinema.Logic.DTOs.Reviews;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class ReviewService : IReviewService
{
    private readonly OnlineCinemaDbContext _dbContext;
    private readonly INotificationService _notificationService;

    public ReviewService(OnlineCinemaDbContext dbContext, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
    }

    public async Task<ReviewResponse> CreateAsync(Guid userId, CreateReviewRequest request)
    {
        var alreadyExists = await _dbContext.Reviews
            .AnyAsync(r => r.UserId == userId && r.ContentId == request.ContentId);

        if (alreadyExists)
        {
            throw new ConflictException("Ви вже залишили відгук на цей контент");
        }

        var review = new Review
        {
            UserId = userId,
            ContentId = request.ContentId,
            Rating = request.Rating,
            Text = request.Text,
            Status = ReviewStatus.Pending
        };

        _dbContext.Reviews.Add(review);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(review);
    }

    public async Task<ReviewResponse> UpdateAsync(Guid userId, Guid reviewId, UpdateReviewRequest request)
    {
        var review = await FindReviewOrThrowAsync(reviewId);

        if (review.UserId != userId)
        {
            throw new ForbiddenException("Ви можете редагувати лише власні відгуки");
        }

        review.Rating = request.Rating;
        review.Text = request.Text;
        review.Status = ReviewStatus.Pending;
        review.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return MapToResponse(review);
    }

    public async Task DeleteAsync(Guid userId, Guid reviewId)
    {
        var review = await FindReviewOrThrowAsync(reviewId);

        if (review.UserId != userId)
        {
            throw new ForbiddenException("Ви можете видаляти лише власні відгуки");
        }

        _dbContext.Reviews.Remove(review);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ReviewResponse>> GetForContentAsync(string contentId)
    {
        var reviews = await _dbContext.Reviews
            .Where(r => r.ContentId == contentId && r.Status == ReviewStatus.Approved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<ReviewResponse>> GetMyReviewsAsync(Guid userId)
    {
        var reviews = await _dbContext.Reviews
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<ReviewResponse>> GetPendingAsync()
    {
        var reviews = await _dbContext.Reviews
            .Where(r => r.Status == ReviewStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(MapToResponse).ToList();
    }

    public async Task<ReviewResponse> ModerateAsync(Guid reviewId, ModerateReviewRequest request)
    {
        if (request.Status != ReviewStatus.Approved && request.Status != ReviewStatus.Rejected)
        {
            throw new ConflictException("Модерувати відгук можна лише зі статусом Approved або Rejected");
        }

        var review = await FindReviewOrThrowAsync(reviewId);

        review.Status = request.Status;
        review.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var statusText = request.Status == ReviewStatus.Approved ? "схвалено" : "відхилено";
        await _notificationService.CreateAsync(
            review.UserId,
            NotificationType.ReviewReply,
            "Ваш відгук перевірено",
            $"Ваш відгук на контент {review.ContentId} було {statusText}.");

        return MapToResponse(review);
    }

    private async Task<Review> FindReviewOrThrowAsync(Guid reviewId)
    {
        var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review == null)
        {
            throw new NotFoundException("Відгук не знайдено");
        }

        return review;
    }

    private static ReviewResponse MapToResponse(Review review)
    {
        return new ReviewResponse
        {
            Id = review.Id,
            UserId = review.UserId,
            ContentId = review.ContentId,
            Rating = review.Rating,
            Text = review.Text,
            Status = review.Status,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}
