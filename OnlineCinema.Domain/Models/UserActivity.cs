using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Domain.Models
{
    public class UserActivity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public ActionType ActionType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Metadata { get; set; }
    }
}
