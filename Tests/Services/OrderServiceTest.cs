using API.Data;
using API.Dtos;
using API.Entity;
using API.Services.impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Tests.Services;

[TestFixture]
public class OrderServiceTest
{
    
    private OrderService _orderService;
    private ApplicationDbContext _dbContext;

    [SetUp]
    public void SetUp()
    {
        _dbContext = GetInMemoryDbContext();
        _orderService = new OrderService(_dbContext, new MemoryCache(new MemoryCacheOptions()));
    }

    [TearDown]
    public void Destroy()
    {
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task Create()
    {
        var customer = new Customer() { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        var serviceResult = await _orderService.Create(new OrderDto(customer.Id));
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.GreaterThan(0));
            Assert.That(result.CustomerId, Is.GreaterThan(0));
        });
    }
    
    [Test]
    public async Task Update()
    {
        var customer1 = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        _dbContext.Customers.Add(customer1);
        var customer2 = new Customer { Name = "Name123", Email = "Email123@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        _dbContext.Customers.Add(customer1);
        await _dbContext.SaveChangesAsync();

        await _orderService.Create(new OrderDto(customer1.Id));
        var serviceResult = await _orderService.Update(1, new OrderDto(customer2.Id));
        
        Assert.That(serviceResult.Success, Is.True);
    }

    [Test]
    public async Task FindById()
    {
        var orderItemService = new OrderItemService(_dbContext);

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
        var customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
                
        _dbContext.Orders.Add(order);
        _dbContext.Products.Add(product);
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();
        await orderItemService.Create(new OrderItemDto(order.Id, product.Id, 20));
        await _orderService.Create(new OrderDto(customer.Id));
        
        var serviceResult = await _orderService.FindById(1);
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.GreaterThan(0));
            Assert.That(result.CustomerId, Is.GreaterThan(0));
            Assert.That(result.Total, Is.EqualTo(399.8m));
        });
    }

    [Test]
    public async Task FindByCustomerId()
    {
        var customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();
        await _orderService.Create(new OrderDto(customer.Id));
        
        var serviceResult = await _orderService.FindByCustomerId(customer.Id);
        var result = serviceResult.Data;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Empty);
            Assert.That(result![0].Id, Is.GreaterThan(0));
            Assert.That(result[0].CustomerId, Is.GreaterThan(0));
        });
    }

    [Test]
    public async Task Delete()
    {
        var customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();
        await _orderService.Create(new OrderDto(customer.Id));

        var serviceResult = await _orderService.Delete(1);
        Assert.That(serviceResult.Success, Is.True);
    }
    
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}