using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Repository;
using Application.Services;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

using Moq;

namespace Tests.Services;

[TestFixture]
public class OrderItemServiceTest
{
    private IOrderItemService _orderItemService;
    private Mock<IOrderItemRepository> _mockOrderItemRepository;
    private Mock<IOrderRepository> _mockOrderRepository;
    private Mock<IProductRepository> _mockProductRepository;

    private Order _order;
    private Product _product;
    private OrderItem _orderItem;

    [SetUp]
    public void Setup()
    {
        _mockOrderItemRepository = new Mock<IOrderItemRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _orderItemService = new OrderItemService(_mockOrderItemRepository.Object, _mockOrderRepository.Object,
            _mockProductRepository.Object, new MemoryCache(new MemoryCacheOptions()));

        _order = new Order
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

        _product = new Product
        {
            Id = 1,
            Name = "name",
            Description = "description",
            Price = 19.99m,
            CategoryId = 1,
            Quantity = 25,
            CreatedAt = DateTime.UtcNow,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };

        _orderItem = new OrderItem
        {
            Id = 1,
            ProductId = _product.Id,
            Product = _product,
            OrderId = _order.Id,
            Order = _order,
            Quantity = 24
        };
    }

    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task CreateOrderItem(OrderItemDto orderItemDto)
    {
        _mockOrderRepository.Setup(x => x.FindById(orderItemDto.OrderId)).ReturnsAsync(_order);
        _mockProductRepository.Setup(x => x.FindById(orderItemDto.ProductId)).ReturnsAsync(_product);
        _mockOrderItemRepository.Setup(x => x.Add(It.IsAny<OrderItem>()))
            .ReturnsAsync((OrderItem orderItem) => orderItem);

        OrderItemResponse result = await _orderItemService.Create(orderItemDto);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ProductId, Is.EqualTo(1));
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.That(result.Quantity, Is.EqualTo(24));
            Assert.That(result.SubTotal, Is.GreaterThan(100));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public Task CreateOrderItemShouldThrowNotFoundException(OrderItemDto orderItemDto)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _orderItemService.Create(orderItemDto));

        Assert.That(ex.Message, Does.Not.Empty);
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task Update(OrderItemDto orderItemDto)
    {
        _mockOrderItemRepository.Setup(x => x.FindById(1)).ReturnsAsync(_orderItem);
        _mockProductRepository.Setup(x => x.FindById(1)).ReturnsAsync(_product);

        OrderItemResponse result = await _orderItemService.Update(1, new OrderItemDto(1, 1, 2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ProductId, Is.EqualTo(1));
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.That(result.Quantity, Is.EqualTo(2));
            Assert.That(result.SubTotal, Is.EqualTo(39.98m));
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public Task UpdateShouldThrowNotFoundException(OrderItemDto orderItemDto)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _orderItemService.Update(1, orderItemDto));

        Assert.That(ex.Message, Does.Not.Empty);
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task FindById(OrderItemDto orderItemDto)
    {
        _mockOrderItemRepository.Setup(x => x.FindById(orderItemDto.OrderId)).ReturnsAsync(_orderItem);
        OrderItemResponse result = await _orderItemService.FindById(orderItemDto.OrderId);

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
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public Task FindByIdShouldThrowNotFoundException(int id)
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _orderItemService.FindById(id));

        Assert.That(ex.Message, Is.EqualTo($"Order item with id: {id} not found"));
        return Task.CompletedTask;
    }

    [Test]
    [TestCaseSource(nameof(CreateOrderItemsDto))]
    public async Task FindByOrderId(OrderItemDto orderItemDto)
    {
        _mockOrderItemRepository.Setup(x => x.FindByOrderId(orderItemDto.OrderId)).ReturnsAsync([_orderItem]);
        List<OrderItemResponse> result = await _orderItemService.FindByOrderId(orderItemDto.OrderId);

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
        _mockOrderItemRepository.Setup(x => x.FindByProductId(orderItemDto.ProductId)).ReturnsAsync([_orderItem]);
        List<OrderItemResponse> result = await _orderItemService.FindByProductId(orderItemDto.ProductId);

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
        _mockOrderItemRepository.Setup(x => x.FindById(1)).ReturnsAsync(_orderItem);
        var result = await _orderItemService.Delete(orderItemDto.OrderId);
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
            await _orderItemService.Delete(id));

        Assert.That(ex.Message, Is.EqualTo($"Order item with id: {id} not found"));
        return Task.CompletedTask;
    }

    private static IEnumerable<OrderItemDto> CreateOrderItemsDto()
    {
        yield return new OrderItemDto(1, 1, 24);
    }
}
