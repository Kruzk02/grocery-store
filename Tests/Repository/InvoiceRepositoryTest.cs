using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class InvoiceRepositoryTest
{

    private PostgresFixture _fixture;
    private string _connectionString;

    private Invoice _invoice;

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new PostgresFixture();
        await _fixture.StartAsync();
        _connectionString = _fixture.GetConnectionString();

        await using ApplicationDbContext context = CreateDbContext();
        await context.Database.MigrateAsync();

        Category? category = await context.Categories.FindAsync(1);

        var product = new Product
        {
            Name = "Apple",
            Description = "OK",
            Category = category!
        };

        await context.Products.AddAsync(product);

        var order = new Order
        {
            Id = 1,
            CustomerId = 1,
            CreatedAt = DateTime.UtcNow,
            Customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "841231245", Address = "asap" },
            Items = [new OrderItem
                {
                    Product = product
                }
            ]
        };
        context.Orders.Add(order);

        _invoice = new Invoice
        {
            Order = order
        };
        context.Invoices.Add(_invoice);
        await context.SaveChangesAsync();
    }

    [Test]
    public async Task AddShouldReturnInvoice()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new InvoiceRepository(context);

        var order = new Order
        {
            Id = 2,
            CustomerId = 1,
            CreatedAt = DateTime.UtcNow,
            Customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "841231245", Address = "asap" },

        };
        context.Orders.Add(order);

        _invoice = new Invoice
        {
            Order = order
        };

        Invoice result = await repository.Add(_invoice);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Order, Is.Not.Null);
        }
    }

    [Test]
    public async Task FindByIdShouldReturnInvoice()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new InvoiceRepository(context);

        Invoice? result = await repository.FindById(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Order, Is.Not.Null);
        }
    }

    [Test]
    public async Task FindByIdShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new InvoiceRepository(context);

        Invoice? result = await repository.FindById(1000);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindByOrderIdShouldReturnInvoice()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new InvoiceRepository(context);

        Invoice? result = await repository.FindByOrderId(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Order, Is.Not.Null);
        }
    }

    [Test]
    public async Task FindByOrderIdShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var repository = new InvoiceRepository(context);

        Invoice? result = await repository.FindByOrderId(1000);
        Assert.That(result, Is.Null);
    }

    private ApplicationDbContext CreateDbContext()
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
