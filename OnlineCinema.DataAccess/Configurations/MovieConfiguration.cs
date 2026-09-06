using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Title).IsRequired().HasMaxLength(100);
            builder.Property(m => m.ImgUrl).IsRequired().HasMaxLength(200);
            builder.HasOne(m => m.Category).WithMany(c => c.Movies).HasForeignKey(m => m.CategoryId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(m => m.Review).IsRequired().HasPrecision(3, 1);
            builder.Property(m => m.RecommendedAge).IsRequired();
            builder.HasMany(m => m.Genres).WithMany(g => g.Movies);
            builder.Property(m => m.DateRealise).IsRequired();
            builder.Property(m => m.Duration).IsRequired();
            builder.Property(m => m.Likes).IsRequired();
            builder.Property(m => m.Dislikes).IsRequired();
            builder.Property(m => m.Description).IsRequired().HasMaxLength(1000);
            builder.HasMany(m => m.Actors).WithMany(a => a.Movies);
            builder.Property(m => m.Country).IsRequired().HasMaxLength(100);
            builder.HasMany(m => m.AudioTracks).WithMany(a => a.Movies);
            builder.HasMany(m => m.Platforms).WithMany(p => p.Movies);
            builder.HasMany(m => m.Comments).WithOne(c => c.Movie).HasForeignKey(c => c.MovieId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(m => m.Favorites).WithOne(f => f.Movie).HasForeignKey(f => f.MovieId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
