using System;
using System.Collections.Generic;
using System.Text;

using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Domain.Models;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid? SubscriptionId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "UAH";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? ProviderTransactionId { get; set; }
    public string Provider { get; set; } = "mock";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
