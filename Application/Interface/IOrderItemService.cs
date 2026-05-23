using Application.Dtos.Request;
using Application.DTOs.Response;

using Domain.Entity;

namespace Application.Interface;

public interface IOrderItemService
{
    Task<OrderItemResponse> Create(OrderItemDto orderItemDto);
    Task<OrderItemResponse> Update(int id, OrderItemDto orderItemDto);
    Task<OrderItemResponse> FindById(int id);
    Task<List<OrderItemResponse>> FindByOrderId(int orderId);
    Task<List<OrderItemResponse>> FindByProductId(int productId);
    Task<bool> Delete(int id);
}
