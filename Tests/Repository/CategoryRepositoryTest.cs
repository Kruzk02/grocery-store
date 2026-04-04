
using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class CategoryRepositoryTest
{
    private PostgresFixture _fixture;
    private string _connectionString;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _fixture = new PostgresFixture();
        await _fixture.StartAsync();
        _connectionString = _fixture.GetConnectionString();

        await using ApplicationDbContext context = CreateDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    private ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_connectionString)
            .Options);
    }

    [Test]
    public async Task FindAllShouldReturnListOfCategory()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var categoryRepository = new CategoryRepository(context);
        List<Category> result = await categoryRepository.FindAll();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }
    }

    [Test]
    public async Task FindByIdShouldReturnCategoryWhenExist()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var categoryRepository = new CategoryRepository(context);
        Category? result = await categoryRepository.FindById(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Fresh Produce"));
            Assert.That(result.Description, Is.EqualTo("Fruits, vegetables, herbs"));
        }
    }

    [Test]
    public async Task FindByIdShouldThrowExceptionWhenNotExist()
    {
        await using ApplicationDbContext context = CreateDbContext();
        var categoryRepository = new CategoryRepository(context);
        Category? result = await categoryRepository.FindById(1000);

        Assert.That(result, Is.Null);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _fixture.StopAsync();
    }
}


