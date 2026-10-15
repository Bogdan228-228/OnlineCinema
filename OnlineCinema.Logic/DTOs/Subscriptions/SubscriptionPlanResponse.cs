using System;

namespace OnlineCinema.Logic.DTOs.Subscriptions;

public class SubscriptionPlanResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = null!;
    public int DurationDays { get; set; }
    public int MaxSimultaneousStreams { get; set; }
    public string MaxQuality { get; set; } = null!;
}
