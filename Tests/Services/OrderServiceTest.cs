using API.Data;
using API.Dtos;
using API.Entity;
using API.Services.impl;
using Microsoft.EntityFrameworkCore;

namespace Tests.Services;

[TestFixture]
public class OrderServiceTest
{
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Test]
    public async Task Create()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderService(ctx);

        var customer = new Customer() { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        ctx.Customers.Add(customer);
        await ctx.SaveChangesAsync();

        var serviceResult = await service.Create(new OrderDto(customer.Id));
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
        var ctx = GetInMemoryDbContext();
        var service = new OrderService(ctx);

        var customer1 = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        ctx.Customers.Add(customer1);
        var customer2 = new Customer { Name = "Name123", Email = "Email123@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        ctx.Customers.Add(customer1);
        await ctx.SaveChangesAsync();

        await service.Create(new OrderDto(customer1.Id));
        var serviceResult = await service.Update(1, new OrderDto(customer2.Id));
        
        Assert.That(serviceResult.Success, Is.True);
    }

    [Test]
    public async Task FindById()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderService(ctx);

        var customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        ctx.Customers.Add(customer);
        await ctx.SaveChangesAsync();
        await service.Create(new OrderDto(customer.Id));
        
        var serviceResult = await service.FindById(1);
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.GreaterThan(0));
            Assert.That(result.CustomerId, Is.GreaterThan(0));
        });
    }

    [Test]
    public async Task FindByCustomerId()
    {
        var ctx = GetInMemoryDbContext();
        var service = new OrderService(ctx);

        var customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        ctx.Customers.Add(customer);
        await ctx.SaveChangesAsync();
        await service.Create(new OrderDto(customer.Id));
        
        var serviceResult = await service.FindByCustomerId(customer.Id);
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
        var ctx = GetInMemoryDbContext();
        var service = new OrderService(ctx);

        var customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3"};
        ctx.Customers.Add(customer);
        await ctx.SaveChangesAsync();
        await service.Create(new OrderDto(customer.Id));

        var serviceResult = await service.Delete(1);
        Assert.That(serviceResult.Success, Is.True);
    }
}