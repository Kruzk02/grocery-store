using Application.Common;
using Application.Queries;

using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class ProductRepositoryTest
{
    private PostgresFixture _fixture;
    private string _connectionString;

    private Product _product;

    [SetUp]
    public async Task SetUp()
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
        await context.SaveChangesAsync();
    }

    [Test]
    public async Task SearchShouldReturnProductsWithNoArguments()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new ProductRepository(context);

        PageResult<Product> result = await repository.Search(new SearchProductQuery(null, 0, null, true));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Total, Is.EqualTo(1));
            Assert.That(result.Data, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public async Task SearchShouldReturnProductsWithArguments()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new ProductRepository(context);

        PageResult<Product> result =
            await repository.Search(new SearchProductQuery("Apple", 0, ProductSortBy.Name, true));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Total, Is.EqualTo(1));
            Assert.That(result.Data, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public async Task AddShouldReturnProduct()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new ProductRepository(context);

        Category? category = await context.Categories.FindAsync(1);

        var product = new Product
        {
            Name = "Apple123",
            Description = "OK",
            Category = category!
        };

        Product result = await repository.Add(product);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(2));
            Assert.That(result.Name, Is.EqualTo("Apple123"));
            Assert.That(result.Description, Is.EqualTo("OK"));
            Assert.That(result.Category, Is.Not.Null);
        }
    }

    [Test]
    public async Task UpdateShouldReturnProduct()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new ProductRepository(context);

        _product.Price = 200m;
        await repository.Update(_product);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_product, Is.Not.Null);
            Assert.That(_product.Id, Is.EqualTo(1));
            Assert.That(_product.Name, Is.EqualTo("Apple"));
            Assert.That(_product.Description, Is.EqualTo("OK"));
            Assert.That(_product.Price, Is.EqualTo(200m));
            Assert.That(_product.Category, Is.Not.Null);
        }
    }

    [Test]
    public async Task FindByIdShouldReturnProduct()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new ProductRepository(context);

        Product? result = await repository.FindById(1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Apple"));
            Assert.That(result.Description, Is.EqualTo("OK"));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new ProductRepository(context);

        Product? result = await repository.FindById(100000);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteShouldDeleteProduct()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new ProductRepository(context);

        await repository.Delete(_product);
        Assert.That(context.Products.Count(), Is.Zero);
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
