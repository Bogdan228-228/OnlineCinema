using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using OnlineCinema.DataAccess;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Notifications;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class NotificationService : INotificationService
{
    private readonly OnlineCinemaDbContext _dbContext;

    public NotificationService(OnlineCinemaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<NotificationResponse>> GetAllAsync(Guid userId, bool unreadOnly)
    {
        var query = _dbContext.Notifications.Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(MapToResponse).ToList();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _dbContext.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var notification = await FindOwnedNotificationAsync(userId, notificationId);

        notification.IsRead = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid userId, Guid notificationId)
    {
        var notification = await FindOwnedNotificationAsync(userId, notificationId);

        _dbContext.Notifications.Remove(notification);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateAsync(Guid userId, NotificationType type, string title, string body)
    {
        _dbContext.Notifications.Add(new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Body = body
        });

        await _dbContext.SaveChangesAsync();
    }

    private async Task<Notification> FindOwnedNotificationAsync(Guid userId, Guid notificationId)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null)
        {
            throw new NotFoundException("Сповіщення не знайдено");
        }

        return notification;
    }

    private static NotificationResponse MapToResponse(Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            Type = notification.Type,
            Title = notification.Title,
            Body = notification.Body,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}
