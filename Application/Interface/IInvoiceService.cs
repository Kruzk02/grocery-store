using Application.Dtos.Request;
using Application.Dtos.Response;

using Domain.Entity;

namespace Application.Interface;

public interface IInvoiceService
{
    Task<InvoiceResponse> Create(InvoiceDto invoiceDto);
    Task<InvoiceResponse> FindById(int id);
    Task<Invoice> FindByOrderId(int orderId);
}
