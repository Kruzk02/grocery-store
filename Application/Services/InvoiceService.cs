using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Repository;

using Domain.Entity;
using Domain.Exception;

namespace Application.Services;

public class InvoiceService(IInvoiceRepository invoiceRepository, IOrderRepository orderRepository) : IInvoiceService
{
    public async Task<InvoiceResponse> Create(InvoiceDto invoiceDto)
    {
        Order? order = await orderRepository.FindById(invoiceDto.OrderId);
        if (order == null) throw new NotFoundException($"Order with id: {invoiceDto.OrderId} not found");

        var invoice = new Invoice
        {
            OrderId = order.Id,
            Order = order,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            InvoiceNumber = $"INV-{DateTime.UtcNow.Year}:{order.Id:D4}"
        };

        return InvoiceResponse.FromEntity(await invoiceRepository.Add(invoice));
    }

    public async Task<InvoiceResponse> FindById(int id)
    {
        Invoice? invoice = await invoiceRepository.FindById(id);
        return InvoiceResponse.FromEntity(invoice ?? throw new NotFoundException($"Invoice with id: {id} not found"));
    }

    public async Task<Invoice> FindByOrderId(int orderId)
    {
        Invoice? invoice = await invoiceRepository.FindByOrderId(orderId);
        return invoice ?? throw new NotFoundException($"Invoice with order id: {orderId} not found");
    }
}
