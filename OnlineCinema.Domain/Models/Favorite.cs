namespace OnlineCinema.Domain.Models;

public class Favorite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public User User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
