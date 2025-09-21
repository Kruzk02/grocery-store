using API.Dtos;
using API.Entity;

namespace API.Services;

public interface IOrderItemService
{
    Task<ServiceResult<OrderItem>> Create(OrderItemDto orderItemDto);
    Task<ServiceResult> Update(int id, OrderItemDto orderItemDto);
    Task<ServiceResult<OrderItem>> FindById(int id);
    Task<ServiceResult<List<OrderItem>>> FindByOrderId(int orderId);
    Task<ServiceResult<List<OrderItem>>> FindByProductId(int productId);
    Task<ServiceResult> Delete(int id);
}