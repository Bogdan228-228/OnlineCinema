using System;
using System.Collections.Generic;
using System.Text;


namespace OnlineCinema.Domain.Enums;

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed,
    Refunded
}
