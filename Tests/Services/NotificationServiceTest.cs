using API.Data;
using API.Entity;
using API.Services.impl;
using Microsoft.EntityFrameworkCore;

namespace Tests.Services;

[TestFixture]
public class NotificationServiceTest
{
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Test]
    public async Task CreateNotification_shouldCreate()
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);

        var notification = new Notification{Id = 1, Message = "adawd", CreatedAt = DateTime.UtcNow, IsRead = false, Type = NotificationType.Info, UserId = "1a"};
        
        var serviceResult = await service.Create(notification);
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Message, Is.EqualTo("adawd"));
            Assert.That(result.IsRead, Is.EqualTo(false));
            Assert.That(result.Type, Is.EqualTo(NotificationType.Info));
        });
    }

    [Test]
    public async Task FindByUserId_shouldReturnNotification()
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);

        var user = new ApplicationUser { Id = "1a"};
        ctx.Users.Add(user);
        
        var notification = new Notification{Id = 1, Message = "adawd", CreatedAt = DateTime.UtcNow, IsRead = false, Type = NotificationType.Info, UserId = "1a"};
        ctx.Notifications.Add(notification);
        
        await ctx.SaveChangesAsync();
        
        var serviceResult = await service.FindByUserId("1a");
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task DeleteById_shouldDelete()
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);

        await service.Create(new Notification
        {
            Id = 1, Message = "adawd", CreatedAt = DateTime.UtcNow, IsRead = false, Type = NotificationType.Info,
            UserId = "1a"
        });

        var serviceResult = await service.DeleteById(1);
        
        Assert.That(serviceResult.Success, Is.True);
    }
    
    [Test]
    public async Task MarkAsRead_shouldMarkAsRead()
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);
        
        await service.Create(new Notification
        {
            Id = 1, Message = "adawd", CreatedAt = DateTime.UtcNow, IsRead = false, Type = NotificationType.Info,
            UserId = "1a"
        });
        
        var serviceResult = await service.MarkAsRead(1);
        Assert.That(serviceResult.Success, Is.True);
    }
    
    [Test]
    public async Task MarkAllAsRead_shouldMarkAllAsRead()
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);
        
        await service.Create(new Notification
        {
            Id = 1, Message = "adawd", CreatedAt = DateTime.UtcNow, IsRead = false, Type = NotificationType.Info,
            UserId = "1a"
        });
        
        var serviceResult = await service.MarkAllAsRead("1a");
        Assert.That(serviceResult.Success, Is.True);
    }
}