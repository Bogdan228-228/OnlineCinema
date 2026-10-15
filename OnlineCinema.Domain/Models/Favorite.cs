namespace OnlineCinema.Domain.Models;

public class Favorite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Колезина (feature/api-bohdan) реалізація: обране прив'язане напряму до Movie.
    public Guid? MovieId { get; set; }
    public Movie? Movie { get; set; }

    // Ваша (Backend Developer 2, ТЗ) реалізація: узагальнене "обране" за ContentId,
    // не прив'язане лише до фільмів.
    public string? ContentId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
