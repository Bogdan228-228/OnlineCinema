using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Configurations
{
    public class AudioTrackConfiguration : IEntityTypeConfiguration<AudioTrack>
    {
        public void Configure(EntityTypeBuilder<AudioTrack> builder)
        {
            builder.HasKey(at => at.Id);
            builder.Property(at => at.Language).IsRequired().HasMaxLength(100);
            builder.HasMany(at => at.Movies).WithMany(m => m.AudioTracks);
        }
    }
}
