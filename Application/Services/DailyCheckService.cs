using Application.Repository;

using Domain.Entity;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class DailyCheckService(
    ILogger<DailyCheckService> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DailyCheckService is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime now = DateTime.UtcNow;
            DateTime scheduledTime = DateTime.UtcNow.Date.AddHours(8);

            if (now > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            TimeSpan delay = scheduledTime - now;
            logger.LogInformation("Next check at {time}", scheduledTime);

            await Task.Delay(delay, stoppingToken);

            using IServiceScope scope = scopeFactory.CreateScope();

            var inventoryRepository = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

            Inventory? inventory = await inventoryRepository.FindLessThanTenQuantity(stoppingToken);
            List<User> adminUsers = await userRepository.GetUserByRole("Admin", stoppingToken);

            foreach (User adminUser in adminUsers)
            {
                if (adminUser.Id == null) continue;
                await notificationRepository.Add(new Notification
                {
                    UserId = adminUser.Id,
                    Message = $"The product quality currently less than 10: {inventory?.Product}",
                    Type = NotificationType.Info,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }, stoppingToken);
            }
        }
    }
}
