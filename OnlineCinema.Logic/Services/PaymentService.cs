using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Payments;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class PaymentService : IPaymentService
{
    private readonly OnlineCinemaDbContext _dbContext;

    public PaymentService(OnlineCinemaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PaymentResponse>> GetMyPaymentsAsync(Guid userId)
    {
        var payments = await _dbContext.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return payments.Select(MapToResponse).ToList();
    }

    public async Task<PaymentResponse> GetByIdAsync(Guid userId, Guid paymentId)
    {
        var payment = await _dbContext.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId);

        if (payment == null)
        {
            throw new NotFoundException("Платіж не знайдено");
        }

        return MapToResponse(payment);
    }

    private static PaymentResponse MapToResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            SubscriptionId = payment.SubscriptionId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status,
            Provider = payment.Provider,
            ProviderTransactionId = payment.ProviderTransactionId,
            CreatedAt = payment.CreatedAt
        };
    }
}
