using API.Data;
using API.Dtos;
using API.Entity;
using API.Services.impl;
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
        var product = new Product{ Id = 1, Name = "name", Description = "description", Price = 11.99m, CategoryId = 1 };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var serviceResult = await _inventoryService.Create(new InventoryDto(product.Id, 20));

        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));   
            Assert.That(result.ProductId, Is.EqualTo(product.Id));
            Assert.That(result.Product, Is.EqualTo(product));
            Assert.That(result.Quantity, Is.EqualTo(20));
        });
    }

    [Test]
    public async Task CreateInventory_shouldFailed_whenProductNotFound()
    {
        var serviceResult = await _inventoryService.Create(new InventoryDto(ProductId: 1, Quantity: 20));
        
        Assert.That(serviceResult.Message, Is.EqualTo("Product not found"));
    }
    
    [Test]
    public async Task UpdateInventory_shouldUpdateInventory()
    {
        var product = new  Product{ Id = 1, Name = "name", Description = "description", Price = 11.99m, CategoryId = 1 };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        
        await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Quantity: 20));
        
        var serviceResult = await _inventoryService.Update(1, new InventoryDto(ProductId: product.Id, Quantity: 10));
        
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult.Message, Is.EqualTo("Inventory updated successfully"));
            Assert.That(serviceResult.Success, Is.True);
        });
    }

    [Test]
    public async Task UpdateInventory_shouldFailed_whenInventoryNotFound()
    {
        var serviceResult = await _inventoryService.Update(1, new InventoryDto(ProductId: 1, Quantity: 20));
        
        Assert.That(serviceResult.Message, Is.EqualTo("Inventory not found"));
    }

    [Test]
    public async Task FindAll_shouldReturnListOfInventory()
    {
        var product = new Product{ Id = 1, Name = "name", Description = "description", Price = 11.99m, CategoryId = 1 };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Quantity: 20));
        
        var serviceResult = await _inventoryService.FindAll();
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task FindById_shouldReturnInventory()
    {
        var product = new Product { Id = 1, Name = "name", Description = "description", Price = 11.99m, CategoryId = 1 };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        
        await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Quantity: 20));

        var serviceResult = await _inventoryService.FindById(1);
        var result = serviceResult.Data;
        
        Assert.Multiple(() => {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.ProductId, Is.EqualTo(product.Id));
            Assert.That(result.Product, Is.EqualTo(product));
            Assert.That(result.Quantity, Is.EqualTo(20));
        });
    }
    
    [Test]
    public async Task FindById_shouldFailed_whenInventoryNotFound()
    {
        var serviceResult = await _inventoryService.FindById(1);
        
        Assert.That(serviceResult.Message, Is.EqualTo("Inventory not found"));
    }
    
    [Test]
    public async Task DeleteById_shouldDeleteInventory()
    {
        var product = new Product { Id = 1, Name = "name", Description = "description", Price = 11.99m, CategoryId = 1 };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        
        await _inventoryService.Create(new InventoryDto(ProductId: product.Id, Quantity: 20));

        var serviceResult = await _inventoryService.Delete(1);
        Assert.That(serviceResult.Success, Is.True);
    }

    [Test]
    public async Task DeleteById_shouldFailed_whenInventoryNotFound()
    {
        var serviceResult = await _inventoryService.Delete(1);
        
        Assert.That(serviceResult.Message, Is.EqualTo("Inventory not found"));
    }
        
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}