using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCinema.Domain.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; } = false;
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
