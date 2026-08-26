using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Domain.Models
{
    public class UserActivity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string? EntityId { get; set; }
        public EntityType EntityType { get; set; } = EntityType.None;
        public ActionType ActionType { get; set; }
        public double Weight { get; set; } = 1.0; // View = 1.0, Like = 2.0, Dislike = -1.0, Favorite = 3.0, Comment = 4.0, Share = 5.0
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Metadata { get; set; }
    }
}
