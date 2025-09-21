using API.Dtos;
using API.Entity;

namespace API.Services;

public interface IOrderService
{
    Task<ServiceResult<Order>> Create(OrderDto orderDto);
    Task<ServiceResult> Update(int id, OrderDto orderDto);
    Task<ServiceResult<Order>> FindById(int id);
    Task<ServiceResult<List<Order>>> FindByCustomerId(int customerId);
    Task<ServiceResult> Delete(int id);
}