using Application.DTOs.Request;
using Application.DTOs.Response;

using Domain.Entity;

namespace Application.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponse> Create(InvoiceDto invoiceDto);
    Task<InvoiceResponse> FindById(int id);
    Task<Invoice> FindByOrderId(int orderId);
}
