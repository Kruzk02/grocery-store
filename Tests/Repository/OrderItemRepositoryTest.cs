using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class OrderItemRepositoryTest
{
    private PostgresFixture _fixture;
    private string _connectionString;

    private OrderItem _orderItem;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new PostgresFixture();
        await _fixture.StartAsync();
        _connectionString = _fixture.GetConnectionString();

        await using ApplicationDbContext context = CreateContext();
        await context.Database.MigrateAsync();
        Category? category = await context.Categories.FindAsync(1);

        var product = new Product
        {
            Name = "Apple",
            Description = "OK",
            Category = category!
        };

        await context.Products.AddAsync(product);

        var customer = new Customer
        {
            Name = "awd",
            Email = "awd@gmail.com",
            Phone = "1234567890",
            Address = "awdiabw"
        };
        await context.Customers.AddAsync(customer);
        var order = new Order
        {
            Customer = customer,
            CustomerId = customer.Id,
        };
        await context.Orders.AddAsync(order);

        _orderItem = new OrderItem
        {
            Product = product,
            Order = order,
            Quantity = 12
        };
        await context.OrderItems.AddAsync(_orderItem);
        await context.SaveChangesAsync();
    }

    [Test]
    public async Task AddShouldReturnOrderItem()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderItemRepository(context);
        Category? category = await context.Categories.FindAsync(1);

        var product = new Product
        {
            Name = "Apple1",
            Description = "OK1",
            Category = category!
        };

        await context.Products.AddAsync(product);

        var customer = new Customer
        {
            Name = "awd1",
            Email = "awd1@gmail.com",
            Phone = "1234567891",
            Address = "awdiabw"
        };
        await context.Customers.AddAsync(customer);
        var order = new Order
        {
            Customer = customer,
            CustomerId = customer.Id,
        };
        await context.Orders.AddAsync(order);

        var orderItem = new OrderItem
        {
            Product = product,
            Order = order,
            Quantity = 12
        };

        OrderItem result = await repository.Add(orderItem);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(2));
            Assert.That(result.Product, Is.Not.Null);
        }
    }

    [Test]
    public async Task UpdateShouldReturnOrderItem()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderItemRepository(context);

        _orderItem.Quantity = 100;
        await repository.Update(_orderItem);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_orderItem, Is.Not.Null);
            Assert.That(_orderItem.Id, Is.EqualTo(1));
            Assert.That(_orderItem.Product, Is.Not.Null);
            Assert.That(_orderItem.Quantity, Is.EqualTo(100));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnOrderItem()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderItemRepository(context);

        OrderItem? result = await repository.FindById(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Quantity, Is.EqualTo(12));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderItemRepository(context);

        OrderItem? result = await repository.FindById(10000);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindByOrderIdShouldReturnListOfOrderItems()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderItemRepository(context);

        List<OrderItem> result = await repository.FindByOrderId(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Product, Is.Not.Null);
            Assert.That(result[0].Order, Is.Not.Null);
            Assert.That(result[0].Quantity, Is.EqualTo(12));
        }
    }

    [Test]
    public async Task FindByProductIdShouldReturnListOfOrderItems()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderItemRepository(context);

        List<OrderItem> result = await repository.FindByProductId(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Product, Is.Not.Null);
            Assert.That(result[0].Quantity, Is.EqualTo(12));
        }
    }

    [Test]
    public async Task DeleteShouldDeleteOrderItem()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderItemRepository(context);

        await repository.Delete(_orderItem);
        Assert.That(context.OrderItems.Count(), Is.EqualTo(0));
    }

    private ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_connectionString)
            .Options);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _fixture.StopAsync();
    }
}
