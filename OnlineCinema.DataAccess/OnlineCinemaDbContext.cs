using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess;

public class OnlineCinemaDbContext : IdentityDbContext<User, Role, Guid>
{
    public OnlineCinemaDbContext(DbContextOptions<OnlineCinemaDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailToken> EmailTokens => Set<EmailToken>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<History> History => Set<History>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Actor> Actors { get; set; }
    public DbSet<AudioTrack> AudioTracks { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Platform> Platforms { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(OnlineCinemaDbContext).Assembly);
    }
}