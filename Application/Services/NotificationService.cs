using Application.DTOs.Response;
using Application.Interfaces;
using Application.Repository;

using Domain.Entity;
using Domain.Exception;

namespace Application.Services;

/// <summary>
/// Provides operations for Create, Retrieve, Delete and mark as read notification.
/// </summary>
/// <remarks>
/// This class interacts with database to performs CRUD operations related to notification.
/// </remarks>
public class NotificationService(INotificationRepository notificationRepository) : INotificationService
{
    /// <inheritdoc />
    public async Task<NotificationResponse> Create(Notification notification)
    {
        return NotificationResponse.FromEntity(await notificationRepository.Add(notification));
    }

    /// <inheritdoc />
    public async Task<List<NotificationResponse>> FindByUserId(string userId)
    {
        List<Notification> notifications = await notificationRepository.FindByUserId(userId);
        return notifications.Select(NotificationResponse.FromEntity).ToList();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteById(int id)
    {
        Notification? notification = await notificationRepository.FindById(id);
        if (notification == null)
        {
            throw new NotFoundException($"Notification with id {id} not found");
        }

        await notificationRepository.Delete(notification);
        return true;
    }

    /// <inheritdoc />
    public async Task<NotificationResponse> MarkAsRead(int id)
    {
        Notification? notification = await notificationRepository.FindById(id);
        return notification == null ? throw new NotFoundException($"Notification with id {id} not found") : NotificationResponse.FromEntity(await notificationRepository.MarkAsRead(notification));
    }

    /// <inheritdoc />
    public async Task<List<NotificationResponse>> MarkAllAsRead(string userId)
    {
        List<Notification> notifications = await notificationRepository.MarkAllAsRead(userId);
        return notifications.Select(NotificationResponse.FromEntity).ToList();
    }
}
