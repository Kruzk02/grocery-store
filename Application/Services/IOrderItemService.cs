﻿using API.Dtos;
using Domain.Entity;

namespace API.Services;

public interface IOrderItemService
{
    Task<OrderItem> Create(OrderItemDto orderItemDto);
    Task<OrderItem> Update(int id, OrderItemDto orderItemDto);
    Task<OrderItem> FindById(int id);
    Task<List<OrderItem>> FindByOrderId(int orderId);
    Task<List<OrderItem>> FindByProductId(int productId);
    Task<bool> Delete(int id);
}