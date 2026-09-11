using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.Logic.DTOs.Subscriptions;

public class SubscribeRequest
{
    [Required]
    public Guid PlanId { get; set; }

    public bool AutoRenew { get; set; } = true;
}
