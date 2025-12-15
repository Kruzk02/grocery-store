using Application.Dtos;
using Application.Services.impl;
using Domain.Entity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Tests.Services;

[TestFixture]
public class InventoryServiceTest
{
    private InventoryService _inventoryService;
    private ApplicationDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        _dbContext = GetInMemoryDbContext();
        _inventoryService = new InventoryService(_dbContext, new MemoryCache(new MemoryCacheOptions()));
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task CreateInventory_shouldCreateProduct()
    {
        var product = new Product
        {
            Id = 1,
            Name = "name",
            Description = "description",
            Price = 11.99m,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var result = await _inventoryService.Create(new InventoryDto(product.Id, 20));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(1));   
            Assert.That(result.ProductId, Is.EqualTo(product.Id));
            Assert.That(result.Product, Is.EqualTo(product));
            Assert.That(result.Quantity, Is.EqualTo(20));
        }
    }
    
    [Test]
    public async Task UpdateInventory_shouldUpdateInventory()
    {
        var product = new  Product
        {
            Id = 1,
            Name = "name",
            Description = "description",
            Price = 11.99m,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        
        await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Quantity: 20));
        
        var result = await _inventoryService.Update(1, new InventoryDto(ProductId: product.Id, Quantity: 10));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(1));   
            Assert.That(result.ProductId, Is.EqualTo(product.Id));
            Assert.That(result.Product, Is.EqualTo(product));
            Assert.That(result.Quantity, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task FindAll_shouldReturnListOfInventory()
    {
        var product = new Product
        {
            Id = 1,
            Name = "name",
            Description = "description",
            Price = 11.99m,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Quantity: 20));
        
        var result = await _inventoryService.FindAll();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public async Task FindById_shouldReturnInventory()
    {
        var product = new Product
        {
            Id = 1,
            Name = "name",
            Description = "description",
            Price = 11.99m,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        
        await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Quantity: 20));

        var result = await _inventoryService.FindById(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ProductId, Is.EqualTo(product.Id));
            Assert.That(result.Product, Is.EqualTo(product));
            Assert.That(result.Quantity, Is.EqualTo(20));
        }
    }
    
    [Test]
    public async Task DeleteById_shouldDeleteInventory()
    {
        var product = new Product
        {
            Id = 1,
            Name = "name",
            Description = "description",
            Price = 11.99m,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        
        await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Quantity: 20));

        var result = await _inventoryService.Delete(1);
        Assert.That(result, Is.EqualTo("Inventory deleted successfully"));
    }
        
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}