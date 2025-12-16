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
    
    private static void CreateProductAndOrder(ApplicationDbContext ctx)
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
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task CreateOrderItem(OrderItemDto orderItemDto)
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();

        var result = await _orderItemService.Create(orderItemDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ProductId, Is.EqualTo(orderItemDto.ProductId));
            Assert.That(result.OrderId, Is.EqualTo(orderItemDto.OrderId));
            Assert.That(result.Quantity, Is.EqualTo(orderItemDto.Quantity));
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
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task FindById(OrderItemDto orderItemDto)
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();
        var orderItem = await _orderItemService.Create(orderItemDto);
        var result = await _orderItemService.FindById(orderItemDto.OrderId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(orderItem.Id));
            Assert.That(result.ProductId, Is.EqualTo(orderItem.ProductId));
            Assert.That(result.OrderId, Is.EqualTo(orderItem.OrderId));
            Assert.That(result.SubTotal, Is.EqualTo(orderItem.SubTotal));
            Assert.That(result.Quantity, Is.EqualTo(orderItem.Quantity));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task FindByOrderId(OrderItemDto orderItemDto)
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();
        var orderItem = await _orderItemService.Create(orderItemDto);
        var result = await _orderItemService.FindByOrderId(orderItemDto.OrderId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, !Is.Empty);
            Assert.That(result, Has.Count.EqualTo(1));
        }
    }
    
    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task FindByProductId(OrderItemDto orderItemDto)
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();
        var orderItem = await _orderItemService.Create(orderItemDto);
        var result = await _orderItemService.FindByProductId(orderItemDto.ProductId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, !Is.Empty);
            Assert.That(result, Has.Count.EqualTo(1));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task Delete(OrderItemDto orderItemDto)
    {
        CreateProductAndOrder(_dbContext);
        await _dbContext.SaveChangesAsync();
        await _orderItemService.Create(orderItemDto);
        var result = await _orderItemService.Delete(orderItemDto.OrderId);
        Assert.That(result, Is.True);
    }

    private static IEnumerable<OrderItemDto> CreateOrderItemsDto()
    {
        yield return new OrderItemDto(1, 1, 24);
    }
    
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}