using Application.Dtos;
using Application.Services.impl;
using Domain.Entity;
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
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task GetAllCustomers(CustomerDto customerDto)
    {
        await _customerService.Create(customerDto);

        var result = await _customerService.FindAll();
        
        Assert.That(result, !Is.Empty);
    }
    
    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task Create(CustomerDto customerDto)
    {
        var result = await _customerService.Create(customerDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.GreaterThan(0));
        }
    }

    [Test]
    public async Task Update()
    {
        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        var result = await _customerService.Update(1, new CustomerDto("Name13", "Emai44l@gmail.com", "843806784", "1b22"));
        
        Assert.That(result, Is.EqualTo("Customer updated successfully"));
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task FindById(CustomerDto customerDto)
    {
        var customer = await _customerService.Create(customerDto);

        var result = await _customerService.FindById(customer.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.Not.Zero);
            Assert.That(result.Name, Is.EqualTo(customer.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Email));
            Assert.That(result.Phone, Is.EqualTo(customer.Phone));
            Assert.That(result.Address, Is.EqualTo(customer.Address));
        }
    }
    
    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task FindByName(CustomerDto customerDto)
    {
        var customer = await _customerService.Create(customerDto);

        var result = await _customerService.FindByName(customer.Name);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.Not.Zero);
            Assert.That(result.Name, Is.EqualTo(customer.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Email));
            Assert.That(result.Phone, Is.EqualTo(customer.Phone));
            Assert.That(result.Address, Is.EqualTo(customer.Address));
        }
    }
    
    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task FindByEmail(CustomerDto customerDto)
    {
        var customer = await _customerService.Create(customerDto);

        var result = await _customerService.FindByEmail(customer.Email);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.Not.Zero);
            Assert.That(result.Name, Is.EqualTo(customer.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Email));
            Assert.That(result.Phone, Is.EqualTo(customer.Phone));
            Assert.That(result.Address, Is.EqualTo(customer.Address));
        }
    }
    
    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task FindByPhoneNumber(CustomerDto customerDto)
    {
        var customer = await _customerService.Create(customerDto);

        var result = await _customerService.FindByPhoneNumber(customer.Phone);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.Not.Zero);
            Assert.That(result.Name, Is.EqualTo(customer.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Email));
            Assert.That(result.Phone, Is.EqualTo(customer.Phone));
            Assert.That(result.Address, Is.EqualTo(customer.Address));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task DeleteById(CustomerDto customerDto)
    {
        var customer = await _customerService.Create(customerDto);

        var result = await _customerService.DeleteById(customer.Id);
        
        Assert.That(result, Is.EqualTo("Customer deleted successfully"));
    }

    private static IEnumerable<CustomerDto> CreateCustomerDto()
    {
        yield return new CustomerDto("Nam1e", "Email1@gmail.com", "843806784", "1b23");
        yield return new CustomerDto("Nam2e", "Email2@gmail.com", "843806894", "1b24");
        yield return new CustomerDto("Name3", "Email3@gmail.com", "843806554", "1b25");
        yield return new CustomerDto("Nam5e", "Email4@gmail.com", "843806424", "1b26");
        yield return new CustomerDto("Name6", "Email5@gmail.com", "843806324", "1b27");
    } 
    
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}