using Application.Common;
using Application.Dtos.Request;
using Application.Interface;
using Application.Queries;
using Application.Repository;
using Application.Services;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

using Moq;

namespace Tests.Services;

[TestFixture]
public class CustomerServiceTest
{
    private ICustomerService _customerService;
    private Mock<ICustomerRepository> _mock;

    [SetUp]
    public void SetUp()
    {
        _mock = new Mock<ICustomerRepository>();
        _customerService = new CustomerService(_mock.Object, new MemoryCache(new MemoryCacheOptions()));
    }

    private static Customer ToEntity(CustomerDto customerDto)
    {
        return new Customer
        {
            Name = customerDto.Name,
            Email = customerDto.Email,
            Phone = customerDto.Phone,
            Address = customerDto.Address
        };
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task SearchCustomerShouldReturnListOfCustomers(CustomerDto customerDto)
    {
        await _customerService.Create(customerDto);
        _mock.Setup(x => x.Search(new SearchCustomerQuery("na", CustomerSortBy.Name, true, 0, 10))).ReturnsAsync(new PageResult<Customer>(1, [ToEntity(customerDto)]));
        (var total, List<Customer> data) = await _customerService.SearchCustomers(new SearchCustomerQuery("na", CustomerSortBy.Name, true, 0));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(total, Is.GreaterThan(0));
            Assert.That(data, Is.Not.Null);
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task CreateSuccess(CustomerDto customerDto)
    {
        _mock.Setup(x => x.Add(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);
        Customer result = await _customerService.Create(customerDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.Not.Null);
            Assert.That(result.Email, Is.Not.Null);
            Assert.That(result.Phone, Is.Not.Null);
            Assert.That(result.Address, Is.Not.Null);
        }
    }

    [TestCase("", "e@mail.com", "123", "addr", "Name")]
    [TestCase("Name", "", "123", "addr", "Email")]
    [TestCase("Name", "e@mail.com", "", "addr", "Phone")]
    [TestCase("Name", "e@mail.com", "123", "", "Address")]
    public Task CreateShouldThrowValidationException(
        string name,
        string email,
        string phone,
        string address,
        string expectedKey)
    {
        var ex = Assert.ThrowsAsync<ValidationException>(async () =>
            await _customerService.Create(new CustomerDto(name, email, phone, address))
        );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex.Errors.ContainsKey(expectedKey), Is.True);
            Assert.That(ex.Errors[expectedKey], Does.Not.Empty);
        }

        return Task.CompletedTask;
    }

    [Test]
    public async Task Update()
    {
        _mock.Setup(x => x.Add(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);

        await _customerService.Create(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22"));

        _mock.Setup(x => x.FindById(1))
            .ReturnsAsync(ToEntity(new CustomerDto("Name", "Email@gmail.com", "843806784", "1b22")));
        _mock.Setup(x => x.Update(It.IsAny<Customer>()));
        var result =
            await _customerService.Update(1, new CustomerDto("Name13", "Emai44l@gmail.com", "843806784", "1b22"));

        Assert.That(result, Is.EqualTo("Customer updated successfully"));
    }

    [Test]
    [TestCase("", "e@mail.com", "123", "addr")]
    [TestCase("Name", "", "123", "addr")]
    [TestCase("Name", "e@mail.com", "", "addr")]
    [TestCase("Name", "e@mail.com", "123", "")]
    public Task UpdateShouldThrowNotFoundException(string name, string email, string phone, string address)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _customerService.Update(1, new CustomerDto(name, email, phone, address)));

        Assert.That(ex.Message, Is.EqualTo($"Customer with id: 1 not found"));

        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task FindById(CustomerDto customerDto)
    {
        _mock.Setup(x => x.Add(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);
        Customer customer = await _customerService.Create(customerDto);

        _mock.Setup(x => x.FindById(1)).ReturnsAsync(ToEntity(customerDto));

        Customer result = await _customerService.FindById(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo(customer.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Email));
            Assert.That(result.Phone, Is.EqualTo(customer.Phone));
            Assert.That(result.Address, Is.EqualTo(customer.Address));
        }
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task FindByIdShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _customerService.FindById(id));
        Assert.That(ex.Message, Is.EqualTo($"Customer with id: {id} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task FindByName(CustomerDto customerDto)
    {
        _mock.Setup(x => x.Add(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);
        Customer customer = await _customerService.Create(customerDto);

        _mock.Setup(x => x.FindByName(customerDto.Name)).ReturnsAsync(ToEntity(customerDto));
        Customer result = await _customerService.FindByName(customer.Name);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo(customer.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Email));
            Assert.That(result.Phone, Is.EqualTo(customer.Phone));
            Assert.That(result.Address, Is.EqualTo(customer.Address));
        }
    }

    [Test]
    [TestCase("we")]
    [TestCase("zx")]
    [TestCase("cd")]
    [TestCase("ff")]
    public Task FindByNameShouldThrowNotFoundException(string name)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _customerService.FindByName(name));
        Assert.That(ex.Message, Is.EqualTo($"Customer with name: {name} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task FindByEmail(CustomerDto customerDto)
    {
        _mock.Setup(x => x.Add(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);
        Customer customer = await _customerService.Create(customerDto);

        _mock.Setup(x => x.FindByEmail(customerDto.Email)).ReturnsAsync(ToEntity(customerDto));
        Customer result = await _customerService.FindByEmail(customer.Email);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo(customer.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Email));
            Assert.That(result.Phone, Is.EqualTo(customer.Phone));
            Assert.That(result.Address, Is.EqualTo(customer.Address));
        }
    }

    [Test]
    [TestCase("we@gmail.com")]
    [TestCase("zx@gmail.com")]
    [TestCase("cd@gmail.com")]
    [TestCase("ff@gmail.com")]
    public Task FindByEmailShouldThrowNotFoundException(string email)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _customerService.FindByEmail(email));
        Assert.That(ex.Message, Is.EqualTo($"Customer with email: {email} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task FindByPhoneNumber(CustomerDto customerDto)
    {
        _mock.Setup(x => x.Add(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);
        Customer customer = await _customerService.Create(customerDto);

        _mock.Setup(x => x.FindByPhoneNumber(customerDto.Phone)).ReturnsAsync(ToEntity(customerDto));
        Customer result = await _customerService.FindByPhoneNumber(customer.Phone);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo(customer.Name));
            Assert.That(result.Email, Is.EqualTo(customer.Email));
            Assert.That(result.Phone, Is.EqualTo(customer.Phone));
            Assert.That(result.Address, Is.EqualTo(customer.Address));
        }
    }

    [Test]
    [TestCase("123")]
    [TestCase("456")]
    [TestCase("789")]
    public Task FindByPhoneShouldThrowNotFoundException(string phone)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _customerService.FindByPhoneNumber(phone));
        Assert.That(ex.Message, Is.EqualTo($"Customer with number: {phone} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomerDto))]
    public async Task DeleteById(CustomerDto customerDto)
    {
        _mock.Setup(x => x.Add(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);
        Customer customer = await _customerService.Create(customerDto);

        _mock.Setup(x => x.FindById(1)).ReturnsAsync(customer);
        _mock.Setup(x => x.Delete(customer));
        var result = await _customerService.DeleteById(1);

        Assert.That(result, Is.EqualTo("Customer deleted successfully"));
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task DeleteByIdShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _customerService.DeleteById(id));
        Assert.That(ex.Message, Is.EqualTo($"Customer with id: {id} not found"));
        return Task.CompletedTask;
    }

    private static IEnumerable<CustomerDto> CreateCustomerDto()
    {
        yield return new CustomerDto("Nam1e", "Email1@gmail.com", "843806784", "1b23");
        yield return new CustomerDto("Nam2e", "Email2@gmail.com", "843806894", "1b24");
        yield return new CustomerDto("Name3", "Email3@gmail.com", "843806554", "1b25");
        yield return new CustomerDto("Nam5e", "Email4@gmail.com", "843806424", "1b26");
        yield return new CustomerDto("Name6", "Email5@gmail.com", "843806324", "1b27");
    }
}
