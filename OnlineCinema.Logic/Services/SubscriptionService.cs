using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Subscriptions;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly OnlineCinemaDbContext _dbContext;
    private readonly INotificationService _notificationService;

    public SubscriptionService(OnlineCinemaDbContext dbContext, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
    }

    public async Task<IReadOnlyList<SubscriptionPlanResponse>> GetPlansAsync()
    {
        var plans = await _dbContext.SubscriptionPlans
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price)
            .ToListAsync();

        return plans.Select(MapPlanToResponse).ToList();
    }

    public async Task<SubscriptionResponse?> GetCurrentAsync(Guid userId)
    {
        var subscription = await _dbContext.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        return subscription == null ? null : MapToResponse(subscription);
    }

    public async Task<IReadOnlyList<SubscriptionResponse>> GetHistoryAsync(Guid userId)
    {
        var subscriptions = await _dbContext.Subscriptions
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return subscriptions.Select(MapToResponse).ToList();
    }

    public async Task<SubscriptionResponse> SubscribeAsync(Guid userId, SubscribeRequest request)
    {
        var plan = await _dbContext.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == request.PlanId && p.IsActive);

        if (plan == null)
        {
            throw new NotFoundException("Тарифний план не знайдено");
        }

        var activeSubscriptions = await _dbContext.Subscriptions
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .ToListAsync();

        foreach (var activeSubscription in activeSubscriptions)
        {
            activeSubscription.Status = SubscriptionStatus.Cancelled;
        }

        var subscription = new Subscription
        {
            UserId = userId,
            PlanId = plan.Id,
            Status = SubscriptionStatus.Active,
            StartsAt = DateTime.UtcNow,
            EndsAt = DateTime.UtcNow.AddDays(plan.DurationDays),
            AutoRenew = request.AutoRenew
        };

        _dbContext.Subscriptions.Add(subscription);

        var payment = new Payment
        {
            UserId = userId,
            SubscriptionId = subscription.Id,
            Amount = plan.Price,
            Currency = plan.Currency,
            Status = PaymentStatus.Succeeded,
            Provider = "mock"
        };

        _dbContext.Payments.Add(payment);

        await _dbContext.SaveChangesAsync();

        await _notificationService.CreateAsync(
            userId,
            NotificationType.NewContent,
            "Підписку активовано",
            $"Ваша підписка \"{plan.Name}\" активна до {subscription.EndsAt:dd.MM.yyyy}.");

        subscription.Plan = plan;
        return MapToResponse(subscription);
    }

    public async Task CancelAsync(Guid userId)
    {
        var subscription = await _dbContext.Subscriptions
            .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (subscription == null)
        {
            throw new NotFoundException("Активної підписки не знайдено");
        }

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.AutoRenew = false;

        await _dbContext.SaveChangesAsync();

        await _notificationService.CreateAsync(
            userId,
            NotificationType.NewContent,
            "Підписку скасовано",
            "Вашу підписку було скасовано.");
    }

    private static SubscriptionResponse MapToResponse(Subscription subscription)
    {
        return new SubscriptionResponse
        {
            Id = subscription.Id,
            Plan = MapPlanToResponse(subscription.Plan),
            Status = subscription.Status,
            StartsAt = subscription.StartsAt,
            EndsAt = subscription.EndsAt,
            AutoRenew = subscription.AutoRenew,
            CreatedAt = subscription.CreatedAt
        };
    }

    private static SubscriptionPlanResponse MapPlanToResponse(SubscriptionPlan plan)
    {
        return new SubscriptionPlanResponse
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            Currency = plan.Currency,
            DurationDays = plan.DurationDays,
            MaxSimultaneousStreams = plan.MaxSimultaneousStreams,
            MaxQuality = plan.MaxQuality
        };
    }
}
