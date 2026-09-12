using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineCinema.Logic.DTOs.Payments;

namespace OnlineCinema.Logic.Interfaces;

public interface IPaymentService
{
    Task<IReadOnlyList<PaymentResponse>> GetMyPaymentsAsync(Guid userId);
    Task<PaymentResponse> GetByIdAsync(Guid userId, Guid paymentId);
}
