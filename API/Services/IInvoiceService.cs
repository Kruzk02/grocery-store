using API.Entity;

namespace API.Services;

public interface IInvoiceService
{
    Task<Invoice> Create(int orderId);
    Task<Invoice> FindById(int id);
    Task<Invoice> FindByOrderId(int orderId);
}