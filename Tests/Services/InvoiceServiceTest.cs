using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interface;
using Application.Repository;
using Application.Services;

using Domain.Entity;
using Domain.Exception;

using Moq;

namespace Tests.Services;

[TestFixture]
public class InvoiceServiceTest
{
    private IInvoiceService _invoiceService;
    private Mock<IInvoiceRepository> _invoiceMock;
    private Mock<IOrderRepository> _orderMock;

    [SetUp]
    public void Setup()
    {
        _invoiceMock = new Mock<IInvoiceRepository>();
        _orderMock = new Mock<IOrderRepository>();
        _invoiceService = new InvoiceService(_invoiceMock.Object, _orderMock.Object);
    }

    [Test]
    public async Task Create()
    {
        var order = new Order
        {
            Id = 1,
            CustomerId = 1,
            CreatedAt = DateTime.UtcNow,
            Customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "841231245", Address = "asap" },
        };

        _orderMock.Setup(x => x.FindById(1)).ReturnsAsync(order);
        _invoiceMock.Setup(x => x.Add(It.IsAny<Invoice>())).ReturnsAsync((Invoice invoice) => invoice);

        InvoiceResponse result = await _invoiceService.Create(new InvoiceDto(1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.OrderId, Is.EqualTo(order.Id));
            Assert.That(result.InvoiceNumber, Is.EqualTo("INV-2026:0001"));
        }
    }

    [Test]
    public Task CreateShouldThrowNotFoundException()
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async ()
            => await _invoiceService.Create(new InvoiceDto(1)));

        Assert.That(ex.Message, Is.EqualTo($"Order with id: 1 not found"));
        return Task.CompletedTask;
    }

    [Test]
    public async Task FindById()
    {
        var customer = new Customer
        {
            Name = "Name",
            Email = "Email@gmail.com",
            Phone = "841231245",
            Address = "asap"
        };

        var product = new Product
        {
            Name = "Sample Product",
            Description = "Description",
            Price = 10m,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };

        var order = new Order
        {
            CustomerId = customer.Id,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 1,
                        Order = new Order
                        {
                            Customer = customer,
                            CreatedAt = DateTime.UtcNow,
                        },
                        Product = product
                    }
            ],
            Customer = new Customer { Name = "Name", Email = "Email@gmail.com", Phone = "841231245", Address = "asap" },
        };


        var invoice = new Invoice
        {
            OrderId = order.Id,
            InvoiceNumber = $"INV-{DateTime.UtcNow.Year}:{order.Id:D4}",
            Order = order
        };

        _invoiceMock.Setup(x => x.FindById(1)).ReturnsAsync(invoice);
        InvoiceResponse result = await _invoiceService.FindById(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.OrderId, Is.EqualTo(order.Id));
            Assert.That(result.InvoiceNumber, Is.EqualTo("INV-2026:0000"));
        }
    }

    [Test]
    public Task FindByIdShouldThrowNotFoundException()
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async ()
            => await _invoiceService.FindById(1));

        Assert.That(ex.Message, Does.Not.Empty);
        return Task.CompletedTask;
    }

    [Test]
    public async Task FindByOrderId()
    {
        var customer = new Customer
        {
            Name = "Name",
            Email = "Email@gmail.com",
            Phone = "841231245",
            Address = "asap"
        };

        var product = new Product
        {
            Name = "Sample Product",
            Description = "Description",
            Price = 10m,
            Category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" }
        };

        var order = new Order
        {
            CustomerId = customer.Id,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    Order = new Order
                    {
                        Customer = customer,
                        CreatedAt = DateTime.UtcNow,
                    },
                    Product = product
                }
            ],
            Customer = customer
        };

        var invoice = new Invoice
        {
            OrderId = order.Id,
            InvoiceNumber = $"INV-{DateTime.UtcNow.Year}:{order.Id:D4}",
            Order = order
        };

        _invoiceMock.Setup(x => x.FindByOrderId(order.Id)).ReturnsAsync(invoice);
        Invoice result = await _invoiceService.FindByOrderId(order.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.OrderId, Is.EqualTo(order.Id));
            Assert.That(result.InvoiceNumber, Is.EqualTo($"INV-{DateTime.UtcNow.Year}:{order.Id:D4}"));
        }
    }

    [Test]
    public Task FindByOrderIdShouldThrowNotFoundException()
    {
        var ex = Assert.ThrowsAsync<NotFoundException>(async ()
            => await _invoiceService.FindByOrderId(1));

        Assert.That(ex.Message, Does.Not.Empty);
        return Task.CompletedTask;
    }
}
