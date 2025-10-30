using API.Data;
using Application.Dtos;
using Application.Services.impl;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Tests.Services;

[TestFixture]
public class CustomerServiceTest
{
    
    private CustomerService _customerService;
    private ApplicationDbContext _dbContext;
    
    [SetUp]
    public void SetUp()
    {
        _dbContext = GetInMemoryDbContext();
        _customerService = new CustomerService(_dbContext, new MemoryCache(new MemoryCacheOptions()));
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();    
    }

    [Test]
    public async Task GetAllCustomers()
    {
        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var result = await _customerService.FindAll();
        
        Assert.That(result, !Is.Empty);
    }
    
    [Test]
    public async Task Create()
    {
        var result = await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));
        
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
        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var result = await _customerService.Update(1, new CustomerDto("Name13", "Emai44l@gmail.com", "843806784", "1b22"));
        
        Assert.That(result, Is.EqualTo("Customer updated successfully"));
    }

    [Test]
    public async Task FindById()
    {
        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var result = await _customerService.FindById(1);
        
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
    public async Task FindByName()
    {
        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var result = await _customerService.FindByName("Name");
        
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
    public async Task FindByEmail()
    {
        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var result = await _customerService.FindByEmail("Email@gmail.com");
        
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
    public async Task FindByPhoneNumber()
    {
        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var result = await _customerService.FindByPhoneNumber("843806784");
        
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
    public async Task DeleteById()
    {
        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var result = await _customerService.DeleteById(1);
        
        Assert.That(result, Is.EqualTo("Customer deleted successfully"));
    }
    
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}