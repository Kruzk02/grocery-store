using API.Dtos;
using Domain.Entity;

namespace API.Services;

public interface IInvoiceService
{
    Task<Invoice> Create(InvoiceDto invoiceDto);
    Task<Invoice> FindById(int id);
    Task<Invoice> FindByOrderId(int orderId);
}