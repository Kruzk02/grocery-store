using Application.Common;
using Application.Queries;

using Domain.Entity;

using Infrastructure.Persistence;
using Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Tests.Repository;

[TestFixture]
public class CustomerRepositoryTest
{

    private PostgresFixture _fixture;
    private string _connectionString;

    private Customer _customer;

    [SetUp]
    public async Task Setup()
    {
        _fixture = new PostgresFixture();
        await _fixture.StartAsync();
        _connectionString = _fixture.GetConnectionString();

        await using ApplicationDbContext context = CreateContext();
        await context.Database.MigrateAsync();

        _customer = new Customer
        {
            Name = "awd",
            Email = "awd@gmail.com",
            Phone = "1234567890",
            Address = "awdiabw"
        };
        await context.Customers.AddAsync(_customer);
        await context.SaveChangesAsync();
    }

    private ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(_connectionString).Options);
    }

    [Test]
    public async Task SearchShouldReturnListOfCustomers()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        PageResult<Customer> result = await repository.Search(new SearchCustomerQuery(null, CustomerSortBy.Name, true, 0));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Total, Is.GreaterThanOrEqualTo(1));
            Assert.That(result.Data, Is.Not.Empty);
        }
    }

    [Test]
    public async Task SaveShouldReturnCustomer()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new CustomerRepository(context);

        Customer result = await repository.Add(new Customer
        {
            Name = "awd123",
            Email = "awd123@gmail.com",
            Phone = "1234567894",
            Address = "awdiabw"
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Is.EqualTo("awd123"));
            Assert.That(result.Email, Is.Not.Null);
            Assert.That(result.Phone, Is.EqualTo("1234567894"));
            Assert.That(result.Address, Is.EqualTo("awdiabw"));
        }
    }

    [Test]
    public async Task UpdateShouldReturnCustomer()
    {
        await using ApplicationDbContext context = CreateContext();
        Customer customer = _customer;

        var repository = new CustomerRepository(context);

        customer.Name = "phuc";
        await repository.Update(customer);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer, Is.Not.Null);
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("phuc"));
            Assert.That(customer.Email, Is.Not.Null);
            Assert.That(customer.Phone, Is.EqualTo("1234567890"));
            Assert.That(customer.Address, Is.EqualTo("awdiabw"));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnCustomer()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        Customer? customer = await repository.FindById(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer, Is.Not.Null);
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("awd"));
            Assert.That(customer.Email, Is.Not.Null);
            Assert.That(customer.Phone, Is.EqualTo("1234567890"));
            Assert.That(customer.Address, Is.EqualTo("awdiabw"));
        }
    }

    [Test]
    public async Task FindByIdShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        Customer? customer = await repository.FindById(100000);
        Assert.That(customer, Is.Null);

    }

    [Test]
    public async Task FindByEmailShouldReturnCustomer()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        Customer? customer = await repository.FindByEmail("awd@gmail.com");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer, Is.Not.Null);
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("awd"));
            Assert.That(customer.Email, Is.Not.Null);
            Assert.That(customer.Phone, Is.EqualTo("1234567890"));
            Assert.That(customer.Address, Is.EqualTo("awdiabw"));
        }
    }

    [Test]
    public async Task FindByEmailShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        Customer? customer = await repository.FindByEmail("123123@gmail.com");
        Assert.That(customer, Is.Null);
    }

    [Test]
    public async Task FindByNameShouldReturnCustomer()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        Customer? customer = await repository.FindByEmail("awd@gmail.com");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer, Is.Not.Null);
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("awd"));
            Assert.That(customer.Email, Is.Not.Null);
            Assert.That(customer.Phone, Is.EqualTo("1234567890"));
            Assert.That(customer.Address, Is.EqualTo("awdiabw"));
        }
    }

    [Test]
    public async Task FindByNameShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        Customer? customer = await repository.FindByName("123123");
        Assert.That(customer, Is.Null);
    }

    [Test]
    public async Task FindByPhoneShouldReturnCustomer()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        Customer? customer = await repository.FindByPhoneNumber("1234567890");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer, Is.Not.Null);
            Assert.That(customer.Id, Is.GreaterThan(0));
            Assert.That(customer.Name, Is.EqualTo("awd"));
            Assert.That(customer.Email, Is.Not.Null);
            Assert.That(customer.Phone, Is.EqualTo("1234567890"));
            Assert.That(customer.Address, Is.EqualTo("awdiabw"));
        }
    }

    [Test]
    public async Task FindByPhoneNumberShouldReturnNull()
    {
        await using ApplicationDbContext context = CreateContext();

        var repository = new CustomerRepository(context);

        Customer? customer = await repository.FindByPhoneNumber("0000000");
        Assert.That(customer, Is.Null);
    }

    [Test]
    public async Task DeleteShouldDeleteCustomer()
    {
        await using ApplicationDbContext context = CreateContext();
        var repository = new CustomerRepository(context);

        await repository.Delete(_customer);
        Assert.That(await context.Customers.CountAsync(), Is.EqualTo(0));
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _fixture.StopAsync();
    }
}
