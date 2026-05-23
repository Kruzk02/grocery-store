using Domain.Entity;

namespace Application.DTOs.Response;

public record NotificationResponse(int Id, NotificationType Type, string Message, bool IsRead, DateTime CreatedAt)
{
    public static NotificationResponse FromEntity(Notification notification)
    {
        return new NotificationResponse(notification.Id, notification.Type, notification.Message, notification.IsRead, notification.CreatedAt);
    }
}
