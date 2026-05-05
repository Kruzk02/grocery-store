using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class OrderRepositoryTest
{
    private PostgresFixture _fixture;
    private string _connectionString;

    private Order _order;
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
        _order = new Order
        {
            Customer = customer,
            CustomerId = customer.Id,
        };
        await context.Orders.AddAsync(_order);
        await context.SaveChangesAsync();
    }

    [Test]
    public async Task AddShouldReturnOrder()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderRepository(context);
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

        Order result = await repository.Add(order);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(2));
            Assert.That(result.CustomerId, Is.EqualTo(order.CustomerId));
            Assert.That(result.Customer, Is.Not.Null);
        }
    }

    [Test]
    public async Task UpdateShouldReturnOrder()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderRepository(context);

        _order.Items.Add(new OrderItem
        {
            Product = new Product
            {
                Name = "Apple1",
                Description = "OK1",
                Category = (await context.Categories.FindAsync(1))!
            },
            Order = _order,
            Quantity = 12
        });
        await repository.Update(_order);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_order, Is.Not.Null);
            Assert.That(_order.Id, Is.EqualTo(1));
            Assert.That(_order.CustomerId, Is.EqualTo(_order.CustomerId));
            Assert.That(_order.Customer, Is.Not.Null);
            Assert.That(_order.Items, Is.Not.Null);
        }
    }

    [Test]
    public async Task FindByIdShouldReturnOrder()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderRepository(context);

        Order? result = await repository.FindById(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.CustomerId, Is.EqualTo(1));
            Assert.That(result.Items, Is.Not.Null);
        }
    }

    [Test]
    public async Task FindByIdShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderRepository(context);

        Order? result = await repository.FindById(1000000);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindByCustomerIdShouldReturnListOfOrder()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderRepository(context);

        List<Order> result = await repository.FindByCustomerId(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].CustomerId, Is.EqualTo(1));
            Assert.That(result[0].Items, Is.Not.Null);
        }
    }

    [Test]
    public async Task DeleteShouldDeleteOrder()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new OrderRepository(context);

        await repository.Delete(_order);
        Assert.That(context.Orders.Count(), Is.EqualTo(0));
    }

    private ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_connectionString).Options);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _fixture.StopAsync();
    }
}
