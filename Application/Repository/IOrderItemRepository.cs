
using Domain.Entity;

namespace Application.Repository;

public interface IOrderItemRepository
{
    Task<OrderItem> Add(OrderItem orderItem);
    Task Update(OrderItem orderItem);
    Task<OrderItem?> FindById(int id);
    Task<List<OrderItem>> FindByOrderId(int orderId);
    Task<List<OrderItem>> FindByProductId(int productId);
    Task Delete(OrderItem orderItem);
}
