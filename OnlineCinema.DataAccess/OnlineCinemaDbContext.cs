using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OnlineCinema.Domain.Models;
using System.Reflection.Emit;
using System.Text.Json;

namespace OnlineCinema.DataAccess;

public class OnlineCinemaDbContext : IdentityDbContext<User, Role, Guid>
{
    public OnlineCinemaDbContext(DbContextOptions<OnlineCinemaDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailToken> EmailTokens => Set<EmailToken>();
    public DbSet<History> History => Set<History>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<AudioTrack> AudioTracks { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<Comment> Comments { get; set; }
   
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Movie>()
       .Property(m => m.SubtitleUrls)
       .HasColumnType("jsonb")
       .HasConversion(
           v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
           v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null)
                ?? new Dictionary<string, string>())
       .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, string>>(
           (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) ==
                     JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
           v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
           v => JsonSerializer.Deserialize<Dictionary<string, string>>(
                    JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    (JsonSerializerOptions?)null)!));

        builder.ApplyConfigurationsFromAssembly(typeof(OnlineCinemaDbContext).Assembly);
    }
}