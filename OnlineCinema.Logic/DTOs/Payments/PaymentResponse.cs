using System;
using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Logic.DTOs.Payments;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public Guid? SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public PaymentStatus Status { get; set; }
    public string Provider { get; set; } = null!;
    public string? ProviderTransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
}
