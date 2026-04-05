using Application.Repository;

using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repository;

public class NotificationRepository(ApplicationDbContext ctx) : INotificationRepository
{
    public async Task<Notification> Add(Notification notification)
    {
        EntityEntry<Notification> result = await ctx.Notifications.AddAsync(notification);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task Add(Notification notification, CancellationToken stoppingToken)
    {
        await ctx.Notifications.AddAsync(notification, stoppingToken);
        await ctx.SaveChangesAsync(stoppingToken);
    }

    public async Task<List<Notification>> FindByUserId(string userId)
    {
        return await ctx.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> FindById(int id)
    {
        return await ctx.Notifications.FindAsync(id);
    }

    public async Task Delete(Notification notification)
    {
        ctx.Notifications.Remove(notification);
        await ctx.SaveChangesAsync();
    }

    public async Task<Notification> MarkAsRead(Notification notification)
    {
        notification.IsRead = true;
        await ctx.SaveChangesAsync();
        return notification;
    }

    public async Task<List<Notification>> MarkAllAsRead(string userId)
    {
        List<Notification> notifications = await ctx.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();
        foreach (Notification n in notifications)
        {
            n.IsRead = true;
        }
        await ctx.SaveChangesAsync();
        return notifications;
    }

}
