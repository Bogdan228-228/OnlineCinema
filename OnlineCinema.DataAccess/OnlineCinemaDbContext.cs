using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess
{
    public class OnlineCinemaDbContext : DbContext
    {
        public OnlineCinemaDbContext(DbContextOptions<OnlineCinemaDbContext> options) : base(options)
        {
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<AudioTrack> AudioTracks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Platform> Platforms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnlineCinemaDbContext).Assembly);
        }
    }

    //public class OnlineCinemaDbContextFactory : IDesignTimeDbContextFactory<OnlineCinemaDbContext>
    //{
    //    public OnlineCinemaDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<OnlineCinemaDbContext>();

    //        optionsBuilder.UseSqlServer(
    //            connectionString: "OnlineCinemaDbContext"
    //        );

    //        return new OnlineCinemaDbContext(optionsBuilder.Options);
    //    }
    //}
}
