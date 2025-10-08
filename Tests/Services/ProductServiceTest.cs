using API.Data;
using API.Dtos;
using API.Entity;
using API.Services.impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Tests.Services;

[TestFixture]
public class ProductServiceTest
{
    private ProductService _productService;
    private ApplicationDbContext _context;
    
    [SetUp]
    public void SetUp()
    {
        _context = GetInMemoryDbContext();
        _productService = new ProductService(_context, new MemoryCache(new MemoryCacheOptions()));
    }

    [TearDown]
    public void Destroy()
    {
        _context.Dispose();
    }
    
    [Test]
    public async Task CreateProduct_ShouldCreateProduct()
    {
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var result = await _productService.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("name"));
            Assert.That(result.Description, Is.EqualTo("description"));
            Assert.That(result.Price, Is.EqualTo(11.99m));
            Assert.That(result.CategoryId, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateProduct_ShouldUpdateProduct()
    {
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        await _productService.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));

        var result = await _productService.Update(1, new ProductDto(Name: "name123", Description: "description123", Price: 11.99m, CategoryId: 1, Quantity: 44));
        
        Assert.Multiple(() =>
        {
            Assert.That(result, !Is.Null);
        });
    }

    [Test]
    public async Task FindAll_shouldReturnListOfProduct()
    {
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        await _productService.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));

        var result = await _productService.FindAll();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task FindById_ShouldReturnProduct()
    {
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        await _productService.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));

        var result = await _productService.FindById(1);
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("name"));
            Assert.That(result.Description, Is.EqualTo("description"));
            Assert.That(result.Price, Is.EqualTo(11.99m));
            Assert.That(result.CategoryId, Is.EqualTo(1));
        });
    }
    
    [Test]
    public async Task DeleteById()
    {
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        await _productService.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));
        
        var result = await _productService.DeleteById(1);
        
        Assert.That(result, Is.True);
    }
    
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}