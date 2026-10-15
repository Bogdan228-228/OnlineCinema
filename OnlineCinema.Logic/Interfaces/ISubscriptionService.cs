using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineCinema.Logic.DTOs.Subscriptions;

namespace OnlineCinema.Logic.Interfaces;

public interface ISubscriptionService
{
    Task<IReadOnlyList<SubscriptionPlanResponse>> GetPlansAsync();
    Task<SubscriptionResponse?> GetCurrentAsync(Guid userId);
    Task<IReadOnlyList<SubscriptionResponse>> GetHistoryAsync(Guid userId);
    Task<SubscriptionResponse> SubscribeAsync(Guid userId, SubscribeRequest request);
    Task CancelAsync(Guid userId);
}
