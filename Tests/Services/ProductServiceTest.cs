using API.Data;
using API.Dtos;
using API.Entity;
using API.Services.impl;
using Microsoft.EntityFrameworkCore;

namespace Tests.Services;

[TestFixture]
public class ProductServiceTest
{

    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
    
    [Test]
    public async Task CreateProduct_ShouldCreateProduct()
    {
        var ctx = GetInMemoryDbContext();
        var service = new ProductService(ctx);
        
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        ctx.Categories.Add(category);
        await ctx.SaveChangesAsync();

        var serviceResult = await service.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));

        var result = serviceResult.Data;
        
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
        var ctx = GetInMemoryDbContext();
        var service = new ProductService(ctx);
        
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        ctx.Categories.Add(category);
        await ctx.SaveChangesAsync();

        await service.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));

        var serviceResult = await service.Update(1, new ProductDto(Name: "name123", Description: "description123", Price: 11.99m, CategoryId: 1, Quantity: 44));
        
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult.Message, Is.EqualTo("Product updated successfully"));
            Assert.That(serviceResult.Success, Is.True);
        });
    }

    [Test]
    public async Task FindAll_shouldReturnListOfProduct()
    {
        var ctx = GetInMemoryDbContext();
        var service = new ProductService(ctx);
        
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        ctx.Categories.Add(category);
        await ctx.SaveChangesAsync();

        await service.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));

        var serviceResult = await service.FindAll();
        var result = serviceResult.Data;
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task FindById()
    {
        var ctx = GetInMemoryDbContext();
        var service = new ProductService(ctx);
        
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        ctx.Categories.Add(category);
        await ctx.SaveChangesAsync();

        await service.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));

        var serviceResult = await service.FindById(1);
        var result = serviceResult.Data;
        
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
        var ctx = GetInMemoryDbContext();
        var service = new ProductService(ctx);
        
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        ctx.Categories.Add(category);
        await ctx.SaveChangesAsync();

        await service.Create(new ProductDto(Name: "name", Description: "description", Price: 11.99m, CategoryId: category.Id, Quantity: 1));
        
        var serviceResult = await service.DeleteById(1);
        
        Assert.That(serviceResult, Is.True);
    }
}