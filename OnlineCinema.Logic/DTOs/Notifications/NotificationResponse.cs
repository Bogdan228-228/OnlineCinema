using System;
using OnlineCinema.Domain.Enums;

namespace OnlineCinema.Logic.DTOs.Notifications;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
