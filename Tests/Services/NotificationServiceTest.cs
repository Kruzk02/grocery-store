using Application.Interface;
using Application.Repository;
using Application.Services;

using Domain.Entity;
using Domain.Exception;

using Moq;

using static NUnit.Framework.Is;

namespace Tests.Services;

[TestFixture]
public class NotificationServiceTest
{
    private INotificationService _notificationService;
    private Mock<INotificationRepository> _mock;

    [SetUp]
    public void Setup()
    {
        _mock = new Mock<INotificationRepository>();
        _notificationService = new NotificationService(_mock.Object);
    }

    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task CreateNotificationShouldCreate(Notification notification)
    {
        _mock.Setup(x => x.Add(It.IsAny<Notification>())).ReturnsAsync(notification);
        Notification result = await _notificationService.Create(notification);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, EqualTo(notification.Id));
            Assert.That(result.Message, EqualTo(notification.Message));
            Assert.That(result.IsRead, False);
            Assert.That(result.Type, EqualTo(NotificationType.Info));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task FindByUserIdShouldReturnNotification(Notification notification)
    {
        _mock.Setup(x => x.FindByUserId(notification.UserId)).ReturnsAsync([notification]);
        List<Notification> result = await _notificationService.FindByUserId(notification.UserId);

        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task DeleteByIdShouldDelete(Notification notification)
    {
        _mock.Setup(x => x.Add(It.IsAny<Notification>())).ReturnsAsync(notification);
        await _notificationService.Create(notification);

        _mock.Setup(x => x.FindById(notification.Id)).ReturnsAsync(notification);
        var serviceResult = await _notificationService.DeleteById(notification.Id);

        Assert.That(serviceResult, EqualTo("Notification Deleted Successfully"));
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task DeleteByIdShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _notificationService.DeleteById(id));

        Assert.That(ex.Message, EqualTo($"Notification with id {id} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task MarkAsReadShouldMarkAsRead(Notification notification)
    {
        _mock.Setup(x => x.FindById(notification.Id)).ReturnsAsync(notification);
        _mock.Setup(x => x.MarkAsRead(notification)).ReturnsAsync(notification);
        Notification result = await _notificationService.MarkAsRead(notification.Id);
        Assert.That(result.IsRead, Not.Null);
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task MarkAsReadShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _notificationService.MarkAsRead(id));

        Assert.That(ex.Message, EqualTo($"Notification with id {id} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateNotification))]
    public async Task MarkAllAsReadShouldMarkAllAsRead(Notification notification)
    {
        _mock.Setup(x => x.FindById(notification.Id)).ReturnsAsync(notification);
        _mock.Setup(x => x.MarkAllAsRead(notification.UserId)).ReturnsAsync([notification]);
        List<Notification> result = await _notificationService.MarkAllAsRead("1a");
        Assert.That(result, !Empty);
    }

    private static IEnumerable<Notification> CreateNotification()
    {
        yield return new Notification
        {
            Id = 1,
            Message = "asap",
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
            Type = NotificationType.Info,
            UserId = "1a"
        };

        yield return new Notification
        {
            Id = 2,
            Message = "asap444",
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
            Type = NotificationType.Info,
            UserId = "1a"
        };

        yield return new Notification
        {
            Id = 3,
            Message = "asap555",
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
            Type = NotificationType.Info,
            UserId = "1a"
        };
    }
}
