using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Logic.DTOs.Notifications;

namespace OnlineCinema.Logic.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationResponse>> GetAllAsync(Guid userId, bool unreadOnly);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid userId, Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteAsync(Guid userId, Guid notificationId);
    Task CreateAsync(Guid userId, NotificationType type, string title, string body);
}
