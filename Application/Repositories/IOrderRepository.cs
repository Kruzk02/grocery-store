
using Domain.Entity;

namespace Application.Repositories;

public interface IOrderRepository
{
    Task<Order> Add(Order order);
    Task Update(Order order);
    Task<Order?> FindById(int id);
    Task<List<Order>> FindByCustomerId(int customerId);
    Task Delete(Order order);
}
