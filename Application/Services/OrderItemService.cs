using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Repositories;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class OrderItemService(IOrderItemRepository orderItemRepository, IOrderRepository orderRepository, IProductRepository productRepository, IMemoryCache cache) : IOrderItemService
{
    public async Task<OrderItemResponse> Create(OrderItemDto orderItemDto)
    {
        Order? order = await orderRepository.FindById(orderItemDto.OrderId);
        if (order == null)
        {
            throw new NotFoundException($"Order with id {orderItemDto.OrderId} not found");
        }

        Product? product = await productRepository.FindById(orderItemDto.ProductId);
        if (product == null)
        {
            throw new NotFoundException($"Product with id {orderItemDto.ProductId} not found");
        }

        var quantity = orderItemDto.Quantity;
        if (quantity <= 0)
        {
            throw new ValidationException(new Dictionary<string, string[]> { { "Quantity", ["Quantity is negative or zero"] } });
        }

        if (product.Quantity < quantity)
        {
            throw new ValidationException(new Dictionary<string, string[]> { { "Quantity", ["Insufficient stock"] } });
        }

        product.Quantity -= quantity;

        var orderItem = new OrderItem
        {
            OrderId = orderItemDto.OrderId,
            Order = order,
            ProductId = orderItemDto.ProductId,
            Product = product,
            Quantity = orderItemDto.Quantity,
        };
        return OrderItemResponse.FromEntity(await orderItemRepository.Add(orderItem));
    }

    public async Task<OrderItemResponse> Update(int id, OrderItemDto orderItemDto)
    {
        OrderItem? orderItem = await orderItemRepository.FindById(id);
        if (orderItem == null)
        {
            throw new NotFoundException($"Order item with id {id} not found");
        }

        if (orderItem.ProductId != orderItemDto.ProductId)
        {
            Product? product = await productRepository.FindById(orderItemDto.ProductId);
            if (product == null)
            {
                throw new NotFoundException($"Product with id {orderItemDto.ProductId} not found");
            }
            orderItem.ProductId = orderItemDto.ProductId;
        }

        if (orderItem.OrderId != orderItemDto.OrderId)
        {
            throw new ValidationException(new Dictionary<string, string[]> { { "OrderId", ["You cannot change the order"] } });
        }

        if (orderItem.Quantity != orderItemDto.Quantity && orderItemDto.Quantity >= 0)
        {
            Product? product = await productRepository.FindById(orderItem.ProductId);
            if (product == null)
            {
                throw new NotFoundException($"Product with id {orderItem.ProductId} not found");
            }

            var availableStock = product.Quantity + orderItem.Quantity;

            if (availableStock < orderItemDto.Quantity)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                    { { "Quantity", ["Insufficient stock"] } });
            }

            product.Quantity += orderItem.Quantity;
            orderItem.Quantity = orderItemDto.Quantity;
            product.Quantity -= orderItem.Quantity;
        }

        await orderItemRepository.Update(orderItem);

        return OrderItemResponse.FromEntity(orderItem);
    }

    public async Task<OrderItemResponse> FindById(int id)
    {
        var cacheKey = $"orderItem:{id}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            OrderItem? orderItem = await orderItemRepository.FindById(id);

            return OrderItemResponse.FromEntity(orderItem ?? throw new NotFoundException($"Order item with id: {id} not found"));
        }) ?? throw new InvalidOperationException();
    }

    public async Task<List<OrderItemResponse>> FindByOrderId(int orderId)
    {
        var cacheKey = $"order:{orderId}:orderItem";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            List<OrderItem> orderItems = await orderItemRepository.FindByOrderId(orderId);

            return orderItems.Select(OrderItemResponse.FromEntity).ToList();
        }) ?? throw new InvalidOperationException();
    }

    public async Task<List<OrderItemResponse>> FindByProductId(int productId)
    {
        var cacheKey = $"product:{productId}:orderItem";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            List<OrderItem> orderItems = await orderItemRepository.FindByProductId(productId);

            return orderItems.Select(OrderItemResponse.FromEntity).ToList();
        }) ?? throw new InvalidOperationException();
    }

    public async Task<bool> Delete(int id)
    {
        OrderItem? orderItem = await orderItemRepository.FindById(id);
        if (orderItem == null)
        {
            throw new NotFoundException($"Order item with id: {id} not found");
        }

        cache.Remove($"orderItem:{id}");

        await orderItemRepository.Delete(orderItem);
        return true;
    }
}
