using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class NotificationRepositoryTest
{
    private PostgresFixture _fixture;
    private string _connectionString;

    private Notification _notification;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new PostgresFixture();
        await _fixture.StartAsync();
        _connectionString = _fixture.GetConnectionString();

        await using ApplicationDbContext context = CreateDbContext();
        await context.Database.MigrateAsync();

        _notification = new Notification
        {
            UserId = "userId",
            Message = "Hello world"
        };
        context.Notifications.Add(_notification);
        await context.SaveChangesAsync();
    }

    [Test]
    public async Task AddShouldReturnNotification()
    {
        await using ApplicationDbContext context = CreateDbContext();

        var repository = new NotificationRepository(context);

        Notification result = await repository.Add(new Notification
        {
            UserId = "userId1",
            Message = "Hello world"
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo("userId1"));
            Assert.That(result.Message, Is.EqualTo("Hello world"));
        }
    }

    [Test]
    public async Task FindByUserIdShouldReturnListOfNotification()
    {
        await using ApplicationDbContext context = CreateDbContext();

        var repository = new NotificationRepository(context);

        List<Notification> result = await repository.FindByUserId("userId");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnNotification()
    {
        await using ApplicationDbContext context = CreateDbContext();

        var repository = new NotificationRepository(context);

        Notification? result = await repository.FindById(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.UserId, Is.EqualTo("userId"));
            Assert.That(result.Message, Is.EqualTo("Hello world"));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateDbContext();

        var repository = new NotificationRepository(context);

        Notification? result = await repository.FindById(1000000);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task MarkAsReadShouldReturnNotificationWithIsRead()
    {
        await using ApplicationDbContext context = CreateDbContext();

        var repository = new NotificationRepository(context);

        Notification result = await repository.MarkAsRead(_notification);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.UserId, Is.EqualTo("userId"));
            Assert.That(result.Message, Is.EqualTo("Hello world"));
            Assert.That(result.IsRead, Is.True);
        }
    }

    [Test]
    public async Task MarkAllAsReadShouldReturnListOfNotificationWithIsRead()
    {
        await using ApplicationDbContext context = CreateDbContext();

        var repository = new NotificationRepository(context);

        List<Notification> result = await repository.MarkAllAsRead("userId");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].UserId, Is.EqualTo("userId"));
            Assert.That(result[0].Message, Is.EqualTo("Hello world"));
            Assert.That(result[0].IsRead, Is.True);
        }
    }

    [Test]
    public async Task DeleteShouldDeleteNotification()
    {
        await using ApplicationDbContext context = CreateDbContext();

        var repository = new NotificationRepository(context);

        await repository.Delete(_notification);
        Assert.That(await context.Notifications.CountAsync(), Is.Zero);
    }

    private ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_connectionString)
            .Options);
    }

    [OneTimeTearDown]
    public async Task TearDOwn()
    {
        await _fixture.StopAsync();
    }
}
