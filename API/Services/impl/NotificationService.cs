using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Services.impl;

/// <summary>
/// Provides operations for Create, Retrieve, Delete and mark as read notification.
/// </summary>
/// <remarks>
/// This class interacts with database to performs CRUD operations related to notification.
/// </remarks>
/// <param name="ctx">The <see cref="ApplicationDbContext"/> used to access the database.</param>
public class NotificationService(ApplicationDbContext ctx) : INotificationService
{
    /// <inheritdoc />
    public async Task<ServiceResult<Notification>> Create(Notification notification)
    {
        var result = await ctx.Notifications.AddAsync(notification);
        await ctx.SaveChangesAsync();
        
        return ServiceResult<Notification>.Ok(result.Entity, "Notification Created Successfully");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<List<Notification>>> FindByUserId(string userId)
    {
        var notifications = await ctx.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return notifications.Count > 0 ?  ServiceResult<List<Notification>>.Ok(notifications, "Notification retrieve successfully") : 
            ServiceResult<List<Notification>>.Failed("Failed to retrieve notifications");
    }

    /// <inheritdoc />
    public async Task<ServiceResult> DeleteById(int id)
    {
        var notification = await ctx.Notifications.FindAsync(id);
        if (notification == null)
        {
            return ServiceResult.Failed("Notification not found");
        }
        
        ctx.Notifications.Remove(notification);
        await ctx.SaveChangesAsync();
        
        return ServiceResult.Ok("Notification Deleted Successfully");
    }

    /// <inheritdoc />
    public async Task<ServiceResult> MarkAsRead(int id)
    {
        var notification = await ctx.Notifications.FindAsync(id);
        if (notification == null)
        {
            return ServiceResult.Failed("Notification not found");
        }
        
        notification.IsRead = true;
        await ctx.SaveChangesAsync();
        return ServiceResult.Ok("Notification Marked Successfully");
    }

    /// <inheritdoc />
    public async Task<ServiceResult> MarkAllAsRead(string userId)
    {
        var notifications = await ctx.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
        foreach (var n in notifications)
        {
            n.IsRead = true;
        }

        await ctx.SaveChangesAsync();
        return ServiceResult.Ok("All notification Marked Successfully");
    }
}