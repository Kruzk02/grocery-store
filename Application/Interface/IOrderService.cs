using Application.Dtos.Request;
using Application.DTOs.Response;

using Domain.Entity;

namespace Application.Interface;

public interface IOrderService
{
    Task<OrderResponse> Create(OrderDto orderDto);
    Task<OrderResponse> Update(int id, OrderDto orderDto);
    Task<OrderResponse> FindById(int id);
    Task<List<OrderResponse>> FindByCustomerId(int customerId);
    Task<bool> Delete(int id);
}
