using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCinema.Domain.Models;

public class SubscriptionPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "UAH";
    public int DurationDays { get; set; }
    public int MaxSimultaneousStreams { get; set; } = 1;
    public string MaxQuality { get; set; } = "SD";
    public bool IsActive { get; set; } = true;
}