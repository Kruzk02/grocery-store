using API.Data;
using API.Dtos;
using API.Services.impl;
using Microsoft.EntityFrameworkCore;

namespace Tests.Services;

[TestFixture]
public class CustomerServiceTest
{
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Test]
    public async Task GetAllCustomers()
    {
        var ctx = GetInMemoryDbContext();
        var service = new CustomerService(ctx);

        await service.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var serviceResult = await service.FindAll();
        var result = serviceResult.Data;
        
        Assert.That(result, !Is.Empty);
    }
    
    [Test]
    public async Task Create()
    {
        var ctx = GetInMemoryDbContext();
        var service = new CustomerService(ctx);

        var serviceResult = await service.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));
        
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.Not.EqualTo(0));
            Assert.That(result.Name, Is.EqualTo("Name"));
            Assert.That(result.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(result.Phone, Is.EqualTo("843806784"));
            Assert.That(result.Address, Is.EqualTo("1b22"));
        });
    }

    [Test]
    public async Task Update()
    {
        var ctx = GetInMemoryDbContext();
        var service = new CustomerService(ctx);
        await service.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var serviceResult = await service.Update(1, new CustomerDto("Name13", "Emai44l@gmail.com", "843806784", "1b22"));
        
        Assert.That(serviceResult.Success, Is.True);
    }

    [Test]
    public async Task FindById()
    {
        var ctx = GetInMemoryDbContext();
        var service = new CustomerService(ctx);
        await service.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var serviceResult = await service.FindById(1);
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult.Success, Is.True);
            Assert.That(result!.Id, Is.Not.EqualTo(0));
            Assert.That(result.Name, Is.EqualTo("Name"));
            Assert.That(result.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(result.Phone, Is.EqualTo("843806784"));
            Assert.That(result.Address, Is.EqualTo("1b22"));
        });
    }
    
    [Test]
    public async Task FindByName()
    {
        var ctx = GetInMemoryDbContext();
        var service = new CustomerService(ctx);
        await service.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var serviceResult = await service.FindByName("Name");
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult.Success, Is.True);
            Assert.That(result!.Id, Is.Not.EqualTo(0));
            Assert.That(result.Name, Is.EqualTo("Name"));
            Assert.That(result.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(result.Phone, Is.EqualTo("843806784"));
            Assert.That(result.Address, Is.EqualTo("1b22"));
        });
    }
    
    [Test]
    public async Task FindByEmail()
    {
        var ctx = GetInMemoryDbContext();
        var service = new CustomerService(ctx);
        await service.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var serviceResult = await service.FindByEmail("Email@gmail.com");
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult.Success, Is.True);
            Assert.That(result!.Id, Is.Not.EqualTo(0));
            Assert.That(result.Name, Is.EqualTo("Name"));
            Assert.That(result.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(result.Phone, Is.EqualTo("843806784"));
            Assert.That(result.Address, Is.EqualTo("1b22"));
        });
    }
    
    [Test]
    public async Task FindByPhoneNumber()
    {
        var ctx = GetInMemoryDbContext();
        var service = new CustomerService(ctx);
        await service.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var serviceResult = await service.FindByPhoneNumber("843806784");
        var result = serviceResult.Data;
        
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult.Success, Is.True);
            Assert.That(result!.Id, Is.Not.EqualTo(0));
            Assert.That(result.Name, Is.EqualTo("Name"));
            Assert.That(result.Email, Is.EqualTo("Email@gmail.com"));
            Assert.That(result.Phone, Is.EqualTo("843806784"));
            Assert.That(result.Address, Is.EqualTo("1b22"));
        });
    }

    [Test]
    public async Task DeleteById()
    {
        var ctx = GetInMemoryDbContext();
        var service = new CustomerService(ctx);
        await service.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var serviceResult = await service.DeleteById(1);
        
        Assert.That(serviceResult.Success, Is.True);
    }
}