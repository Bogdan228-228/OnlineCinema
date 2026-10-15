using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(100);
        builder.Property(p => p.Price).HasPrecision(10, 2);
        builder.Property(p => p.Currency).HasMaxLength(3);
        builder.Property(p => p.MaxQuality).HasMaxLength(10);

        builder.HasData(
            new SubscriptionPlan
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Basic",
                Price = 99m,
                Currency = "UAH",
                DurationDays = 30,
                MaxSimultaneousStreams = 1,
                MaxQuality = "SD",
                IsActive = true
            },
            new SubscriptionPlan
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Standard",
                Price = 199m,
                Currency = "UAH",
                DurationDays = 30,
                MaxSimultaneousStreams = 2,
                MaxQuality = "HD",
                IsActive = true
            },
            new SubscriptionPlan
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Premium",
                Price = 299m,
                Currency = "UAH",
                DurationDays = 30,
                MaxSimultaneousStreams = 4,
                MaxQuality = "UHD",
                IsActive = true
            });
    }
}
