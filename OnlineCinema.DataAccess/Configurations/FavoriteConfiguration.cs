using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        // MovieId/ContentId навмисно необов'язкові: один запис Favorite може
        // прийти або з фільм-орієнтованого шляху колеги (MovieId), або з
        // узагальненого content-шляху (ContentId) з вашого модуля Favorites.
        builder.HasOne(x => x.User).WithMany(u => u.Favorites).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Movie).WithMany(m => m.Favorites).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.ContentId });
    }
}
