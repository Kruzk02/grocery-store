using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

using Tests.Repository;

namespace Tests.Controller;

public class BaseControllerTest
{
    protected HttpClient Client { get; private set; }

    private PostgresFixture _db;
    private WebApplicationFactory<Program> _factory;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _db = new PostgresFixture();
        await _db.StartAsync();

        var connectionString = _db.GetConnectionString();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    ServiceDescriptor? descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); });

                    services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = "Test";
                            options.DefaultChallengeScheme = "Test";
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                });
            });

        using IServiceScope scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        await db.Notifications.AddAsync(new Notification
        {
            Id = 1,
            UserId = "test-user-id",
            IsRead = false,
            Message = "Meesage"
        });

        await db.Notifications.AddAsync(new Notification
        {
            Id = 2,
            UserId = "test-user-id",
            IsRead = false,
            Message = "Meesage1"
        });

        await db.Products.AddAsync(new Product
        {
            Id = 2,
            Name = "product-name",
            Description = "product-description",
            CategoryId = 1,
            Category = (await db.Categories.FindAsync(1))!,
            Price = 129m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Quantity = 1,
        });

        EntityEntry<Customer> customer = await db.Customers.AddAsync(new Customer
        {
            Id = 2,
            Name = "eeeee",
            Email = "eeeemail@gmail.com",
            Phone = "0223456789",
            Address = "Customer-Address"
        });

        await db.Orders.AddAsync(new Order
        {
            Id = 2,
            Customer = customer.Entity,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        Client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        Client.Dispose();

        await _db.StopAsync();
        await _factory.DisposeAsync();
    }
}
