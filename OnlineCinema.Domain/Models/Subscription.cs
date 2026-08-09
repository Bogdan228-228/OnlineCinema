using System;
using System.Collections.Generic;
using System.Text;

using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Domain.Models;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }
    public SubscriptionPlan Plan { get; set; } = null!;

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public bool AutoRenew { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}