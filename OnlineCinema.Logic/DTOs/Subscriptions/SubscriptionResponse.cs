using System;
using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Logic.DTOs.Subscriptions;

public class SubscriptionResponse
{
    public Guid Id { get; set; }
    public SubscriptionPlanResponse Plan { get; set; } = null!;
    public SubscriptionStatus Status { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public bool AutoRenew { get; set; }
    public DateTime CreatedAt { get; set; }
}
