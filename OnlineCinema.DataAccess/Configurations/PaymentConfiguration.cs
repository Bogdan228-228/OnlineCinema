using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasIndex(p => p.UserId);
        builder.Property(p => p.Amount).HasPrecision(10, 2);
        builder.Property(p => p.Currency).HasMaxLength(3);
        builder.Property(p => p.Provider).HasMaxLength(50);
        builder.Property(p => p.ProviderTransactionId).HasMaxLength(200);
    }
}
