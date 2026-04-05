using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class InventoryRepositoryTest
{
    private PostgresFixture _fixture;
    private string _connectionString;

    private Inventory _inventory;
    private Product _product;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new PostgresFixture();
        await _fixture.StartAsync();
        _connectionString = _fixture.GetConnectionString();

        await using ApplicationDbContext context = CreateContext();
        await context.Database.MigrateAsync();

        Category? category = await context.Categories.FindAsync(1);

        _product = new Product
        {
            Name = "Apple",
            Description = "OK",
            Category = category!
        };

        await context.Products.AddAsync(_product);

        _inventory = new Inventory
        {
            Product = _product,
            Stock = 10,
        };
        await context.Inventories.AddAsync(_inventory);
        await context.SaveChangesAsync();
    }

    private ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_connectionString)
            .Options);
    }

    [Test]
    public async Task FindAllShouldReturnListOfInventoryWithNoArgument()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        (int total, List<Inventory> data) result = await repository.FindAll(null, null, null, 0, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.total, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.data, Is.Not.Empty);
        }
    }

    [Test]
    public async Task FindAllShouldReturnListOfInventoryWithProductIdArgument()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        (int total, List<Inventory> data) result = await repository.FindAll(1, null, null, 0, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.total, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.data, Is.Not.Empty);
        }
    }

    [Test]
    public async Task FindAllShouldReturnListOfInventoryWithStockArgument()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        (int total, List<Inventory> data) result = await repository.FindAll(null, 10, null, 0, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.total, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.data, Is.Not.Empty);
        }
    }

    [Test]
    public async Task FindAllShouldReturnListOfInventoryWithProductNameArgument()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        (int total, List<Inventory> data) result = await repository.FindAll(null, null, "Apple", 0, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.total, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.data, Is.Not.Empty);
        }
    }

    [Test]
    public async Task FindAllShouldReturnListOfInventoryWithAllArgument()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        (int total, List<Inventory> data) result = await repository.FindAll(1, 10, "Apple", 0, 10);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.total, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.data, Is.Not.Empty);
        }
    }

    [Test]
    public async Task AddShouldReturnInventory()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        Inventory result = await repository.Add(new Inventory
        {
            Product = new Product
            {
                Name = "Apple",
                Description = "OK",
                Category = new Category
                {
                    Name = "awd",
                    Description = "zxc"
                }
            },
            Stock = 20,
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(1));
            Assert.That(result.Product, Is.Not.Null);
            Assert.That(result.Stock, Is.EqualTo(20));
        }
    }

    [Test]
    public async Task UpdateShouldReturnInventory()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        _inventory.Stock = 9;
        await repository.Update(_inventory);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_inventory, Is.Not.Null);
            Assert.That(_inventory.Id, Is.EqualTo(1));
            Assert.That(_inventory.Product, Is.Not.Null);
            Assert.That(_inventory.Stock, Is.EqualTo(9));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnInventory()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        Inventory? result = await repository.FindById(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Product, Is.Null); // Is null because not include product in repository.
            Assert.That(result.Stock, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        Inventory? result = await repository.FindById(10000);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FindLessThanTenQuantityShouldReturnInventory()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        Inventory? result = await repository.FindLessThanTenQuantity(CancellationToken.None);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Stock, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task FindLessThanTenQuantityShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);
        _inventory.Stock = 11;
        await repository.Update(_inventory);

        Inventory? result = await repository.FindLessThanTenQuantity(CancellationToken.None);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Delete()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new InventoryRepository(context);

        await repository.Delete(_inventory);
        Assert.That(await context.Inventories.CountAsync(), Is.Zero);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _fixture.StopAsync();
    }
}
