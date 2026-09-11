using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Configurations
{
    public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
    {
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder.HasKey(ua => ua.Id);
            builder.HasOne(ua => ua.User).WithMany(u => u.UserActivities).HasForeignKey(ua => ua.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Property(ua => ua.EntityId);
            builder.Property(ua => ua.EntityType).IsRequired();
            builder.Property(ua => ua.ActionType).IsRequired();
            builder.Property(ua => ua.Timestamp).IsRequired();
            builder.Property(ua => ua.Metadata).HasMaxLength(1000);
        }
    }
}
