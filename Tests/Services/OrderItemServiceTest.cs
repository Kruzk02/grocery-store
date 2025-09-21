using API.Data;
using API.Dtos;
using API.Entity;
using API.Services.impl;
using Microsoft.EntityFrameworkCore;

namespace Tests.Services;

[TestFixture]
public class OrderItemServiceTest
{
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private void CreateProductAndOrder(ApplicationDbContext ctx)
    {
        var order = new Order
        {
            Id = 1,
            CustomerId = 1,
            CreatedAt = DateTime.UtcNow,
        };

        var product = new Product
        {
            Id = 1,
            Name = "name",
            Description = "description",
            Price = 19.99m,
            CategoryId = 1,
            Quantity = 20,
            CreatedAt = DateTime.UtcNow,
        };
        
        ctx.Orders.Add(order);
        ctx.Products.Add(product);
    }

    [Test]
    public async Task CreateOrderItem()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderItemService(ctx);

        CreateProductAndOrder(ctx);
        await ctx.SaveChangesAsync();

        var serviceResult = await service.Create(new OrderItemDto(1, 1, 24));
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.ProductId, Is.EqualTo(1));
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.That(result.SubTotal, Is.EqualTo(479.76m));
            Assert.That(result.Quantity, Is.EqualTo(24));
        });
    }

    [Test]
    public async Task Update()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderItemService(ctx);

        CreateProductAndOrder(ctx);
        
        await ctx.SaveChangesAsync();
        await service.Create(new OrderItemDto(1, 1, 24));
        var serviceResult = await service.Update(1, new OrderItemDto(1, 1, 13));
        Assert.That(serviceResult.Success, Is.True);        
    }

    [Test]
    public async Task FindById()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderItemService(ctx);
        
        CreateProductAndOrder(ctx);
        await ctx.SaveChangesAsync();
        await service.Create(new OrderItemDto(1, 1, 24));
        var serviceResult = await service.FindById(1);
        var result = serviceResult.Data;
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.ProductId, Is.EqualTo(1));
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.That(result.SubTotal, Is.EqualTo(479.76m));
            Assert.That(result.Quantity, Is.EqualTo(24));
        });
    }

    [Test]
    public async Task FindByOrderId()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderItemService(ctx);
        
        CreateProductAndOrder(ctx);
        await ctx.SaveChangesAsync();
        await service.Create(new OrderItemDto(1, 1, 24));
        var serviceResult = await service.FindByOrderId(1);
        var result = serviceResult.Data;
        Assert.Multiple(() =>
        {
            Assert.That(result, !Is.Empty);
            Assert.That(result![0].Id, Is.EqualTo(1));
            Assert.That(result[0].ProductId, Is.EqualTo(1));
            Assert.That(result[0].OrderId, Is.EqualTo(1));
            Assert.That(result[0].SubTotal, Is.EqualTo(479.76m));
            Assert.That(result[0].Quantity, Is.EqualTo(24));
        });
    }
    
    [Test]
    public async Task FindByProductId()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderItemService(ctx);
        
        CreateProductAndOrder(ctx);
        await ctx.SaveChangesAsync();
        await service.Create(new OrderItemDto(1, 1, 24));
        var serviceResult = await service.FindByProductId(1);
        var result = serviceResult.Data;
        Assert.Multiple(() =>
        {
            Assert.That(result, !Is.Empty);
            Assert.That(result![0].Id, Is.EqualTo(1));
            Assert.That(result[0].ProductId, Is.EqualTo(1));
            Assert.That(result[0].OrderId, Is.EqualTo(1));
            Assert.That(result[0].SubTotal, Is.EqualTo(479.76m));
            Assert.That(result[0].Quantity, Is.EqualTo(24));
        });
    }

    [Test]
    public async Task Delete()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderItemService(ctx);
        
        CreateProductAndOrder(ctx);
        await ctx.SaveChangesAsync();
        await service.Create(new OrderItemDto(1, 1, 24));
        var serviceResult = await service.Delete(1);
        Assert.That(serviceResult.Success, Is.True);
    }
}