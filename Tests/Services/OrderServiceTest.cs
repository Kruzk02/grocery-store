using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

using Moq;

namespace Tests.Services;

[TestFixture]
public class OrderServiceTest
{
    private IOrderService _orderService;
    private Mock<IOrderRepository> _mockOrderRepository;
    private Mock<ICustomerRepository> _mockCustomerRepository;

    private Customer _customer;
    private Order _order;

    [SetUp]
    public void SetUp()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _orderService = new OrderService(_mockOrderRepository.Object, _mockCustomerRepository.Object,
            new MemoryCache(new MemoryCacheOptions()));

        _customer = new Customer
        {
            Id = 1,
            Name = "Name",
            Email = "Email@gmail.com",
            Phone = "841231245",
            Address = "asap"
        };

        _order = new Order
        {
            Id = 1,
            CustomerId = 1,
            CreatedAt = DateTime.UtcNow,
            Customer = _customer
        };
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomer))]
    public async Task Create(Customer customer)
    {
        _mockCustomerRepository.Setup(x => x.FindById(customer.Id)).ReturnsAsync(_customer);
        _mockOrderRepository.Setup(x => x.Add(It.IsAny<Order>())).ReturnsAsync(_order);
        OrderResponse result = await _orderService.Create(new OrderDto(customer.Id));

        Assert.That(result.CustomerId, Is.GreaterThan(0));
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomer))]
    public Task CreateShouldThrowNotFoundException(Customer customer)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _orderService.Create(new OrderDto(customer.Id)));

        Assert.That(ex.Message, Is.EqualTo($"Customer with id {customer.Id} not found"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task Update()
    {
        _mockOrderRepository.Setup(x => x.FindById(1)).ReturnsAsync(_order);
        _mockCustomerRepository.Setup(x => x.FindById(1)).ReturnsAsync(_customer);
        OrderResponse result = await _orderService.Update(1, new OrderDto(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, !Is.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.CustomerId, Is.EqualTo(1));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomer))]
    public Task UpdateShouldThrowNotFoundException(Customer customer)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _orderService.Update(1, new OrderDto(customer.Id)));

        Assert.That(ex.Message, Is.EqualTo($"Order with id 1 not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomer))]
    public async Task FindById(Customer customer)
    {
        _mockOrderRepository.Setup(x => x.FindById(1)).ReturnsAsync(_order);
        OrderResponse result = await _orderService.FindById(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.CustomerId, Is.GreaterThan(0));
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
            await _orderService.FindById(id));

        Assert.That(ex.Message, Is.EqualTo($"Order with id: {id} not found."));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomer))]
    public async Task FindByCustomerId(Customer customer)
    {
        _mockOrderRepository.Setup(x => x.FindByCustomerId(customer.Id)).ReturnsAsync([_order]);
        List<OrderResponse> result = await _orderService.FindByCustomerId(customer.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Empty);
            Assert.That(result[0].Id, Is.GreaterThan(0));
            Assert.That(result[0].CustomerId, Is.GreaterThan(0));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateCustomer))]
    public async Task Delete(Customer customer)
    {
        _mockOrderRepository.Setup(x => x.FindById(1)).ReturnsAsync(_order);
        var result = await _orderService.Delete(1);
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task DeleteShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _orderService.Delete(id));

        Assert.That(ex.Message, Is.EqualTo($"Order with id {id} not found"));
        return Task.CompletedTask;
    }

    private static IEnumerable<Customer> CreateCustomer()
    {
        yield return new Customer
            { Name = "Name", Email = "Email@gmail.com", Phone = "84 123 456 78", Address = "2aad3" };
    }
}
