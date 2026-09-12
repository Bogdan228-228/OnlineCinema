using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Configurations;

public class HistoryConfiguration : IEntityTypeConfiguration<History>
{
    public void Configure(EntityTypeBuilder<History> builder)
    {
        builder.HasIndex(h => new { h.UserId, h.ContentId }).IsUnique();
    }
}
