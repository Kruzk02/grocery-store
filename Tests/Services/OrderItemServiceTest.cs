using Application.Dtos;
using Application.Services.impl;
using Domain.Entity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Tests.Services;

[TestFixture]
public class OrderItemServiceTest
{
    
    private OrderItemService _orderItemService;
    private ApplicationDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        _dbContext = GetInMemoryDbContext();
        _orderItemService = new OrderItemService(_dbContext, new MemoryCache(new MemoryCacheOptions()));
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();    
    }
    
    private void CreateProductAndOrder(ApplicationDbContext ctx)
    {
        var order = new Order
        {
            Id = 1,
            CustomerId = 1,
            CreatedAt = DateTime.UtcNow,
            Customer = new Customer
            {
                Name = "Name",
                Email = "Email@gmail.com",
                Phone = "841231245",
                Address = "asap"
            }
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
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };
        
        ctx.Orders.Add(order);
        ctx.Products.Add(product);
    }

    [Test]
    public async Task CreateOrderItem()
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();

        var result = await _orderItemService.Create(new OrderItemDto(1, 1, 24));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ProductId, Is.EqualTo(1));
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.That(result.SubTotal, Is.EqualTo(479.76m));
            Assert.That(result.Quantity, Is.EqualTo(24));
        }
    }

    [Test]
    public async Task Update()
    {
        CreateProductAndOrder(_dbContext);
        
        await _dbContext.SaveChangesAsync();
        await _orderItemService.Create(new OrderItemDto(1, 1, 24));
        var result = await _orderItemService.Update(1, new OrderItemDto(1, 1, 13));
        Assert.That(result, !Is.Null);        
    }

    [Test]
    public async Task FindById()
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();
        await _orderItemService.Create(new OrderItemDto(1, 1, 24));
        var result = await _orderItemService.FindById(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ProductId, Is.EqualTo(1));
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.That(result.SubTotal, Is.EqualTo(479.76m));
            Assert.That(result.Quantity, Is.EqualTo(24));
        }
    }

    [Test]
    public async Task FindByOrderId()
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();
        await _orderItemService.Create(new OrderItemDto(1, 1, 24));
        var result = await _orderItemService.FindByOrderId(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, !Is.Empty);
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].ProductId, Is.EqualTo(1));
            Assert.That(result[0].OrderId, Is.EqualTo(1));
            Assert.That(result[0].SubTotal, Is.EqualTo(479.76m));
            Assert.That(result[0].Quantity, Is.EqualTo(24));
        }
    }
    
    [Test]
    public async Task FindByProductId()
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();
        await _orderItemService.Create(new OrderItemDto(1, 1, 24));
        var result = await _orderItemService.FindByProductId(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, !Is.Empty);
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].ProductId, Is.EqualTo(1));
            Assert.That(result[0].OrderId, Is.EqualTo(1));
            Assert.That(result[0].SubTotal, Is.EqualTo(479.76m));
            Assert.That(result[0].Quantity, Is.EqualTo(24));
        }
    }

    [Test]
    public async Task Delete()
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();
        await _orderItemService.Create(new OrderItemDto(1, 1, 24));
        var result = await _orderItemService.Delete(1);
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