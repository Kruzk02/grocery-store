using Application.Services.impl;
using Domain.Entity;
using Infrastructure.Persistence;
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
    [TestCaseSource(nameof(CreateNotification))]
    public async Task CreateNotification_shouldCreate(Notification notification)
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);
        
        var result = await service.Create(notification);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(notification.Id));
            Assert.That(result.Message, Is.EqualTo(notification.Message));
            Assert.That(result.IsRead, Is.False);
            Assert.That(result.Type, Is.EqualTo(NotificationType.Info));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task FindByUserId_shouldReturnNotification(Notification notification)
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);

        var user = new ApplicationUser { Id = "1a"};
        ctx.Users.Add(user);
        
        ctx.Notifications.Add(notification);
        
        await ctx.SaveChangesAsync();
        
        var result = await service.FindByUserId(notification.UserId);
        
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task DeleteById_shouldDelete(Notification notification)
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);

        await service.Create(notification);

        var serviceResult = await service.DeleteById(notification.Id);
        
        Assert.That(serviceResult, Is.EqualTo("Notification Deleted Successfully"));
    }
    
    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task MarkAsRead_shouldMarkAsRead(Notification notification)
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);
        
        await service.Create(notification);
        
        var result = await service.MarkAsRead(notification.Id);
        Assert.That(result.IsRead, Is.True);
    }
    
    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task MarkAllAsRead_shouldMarkAllAsRead(Notification notification)
    {
        var ctx = GetInMemoryDbContext();
        var service = new NotificationService(ctx);
        
        await service.Create(notification);
        
        var result = await service.MarkAllAsRead("1a");
        Assert.That(result, !Is.Empty);
    }

    private static IEnumerable<Notification> CreateNotification()
    {
        yield return new Notification
        {
            Id = 1, Message = "asap", CreatedAt = DateTime.UtcNow, IsRead = false, Type = NotificationType.Info,
            UserId = "1a"
        };  
        
        yield return new Notification
        {
            Id = 2, Message = "asap444", CreatedAt = DateTime.UtcNow, IsRead = false, Type = NotificationType.Info,
            UserId = "1a"
        };  
        
        yield return new Notification
        {
            Id = 3, Message = "asap555", CreatedAt = DateTime.UtcNow, IsRead = false, Type = NotificationType.Info,
            UserId = "1a"
        };
    }
}