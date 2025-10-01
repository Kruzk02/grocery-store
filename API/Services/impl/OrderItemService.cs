using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services.impl;

public class OrderItemService(ApplicationDbContext ctx, IMemoryCache cache) : IOrderItemService
{
    public async Task<ServiceResult<OrderItem>> Create(OrderItemDto orderItemDto)
    {
        var order = await ctx.Orders.FindAsync(orderItemDto.OrderId);
        if (order == null)
        {
            return ServiceResult<OrderItem>.Failed("Order not found");    
        }

        var product = await ctx.Products.FindAsync(orderItemDto.ProductId);
        if (product == null)
        {
            return ServiceResult<OrderItem>.Failed("Product not found");
        }
        
        var quantity = orderItemDto.Quantity;
        if (quantity <= 0)
        {
            return ServiceResult<OrderItem>.Failed("Quantity is negative or zero");
        }
        
        var orderItem = new OrderItem()
        {
            OrderId = orderItemDto.OrderId,
            Order = order,
            ProductId = orderItemDto.ProductId,
            Product = product,
            Quantity = orderItemDto.Quantity,
        };
        
        var result = await ctx.OrderItems.AddAsync(orderItem);
        await ctx.SaveChangesAsync();
        
        return ServiceResult<OrderItem>.Ok(result.Entity, "Order item created successfully");
    }

    public async Task<ServiceResult> Update(int id, OrderItemDto orderItemDto)
    {
        var orderItem = await ctx.OrderItems.FindAsync(id);
        if (orderItem == null)
        {
            return ServiceResult<OrderItem>.Failed("Order item not found");
        }

        if (orderItem.ProductId != orderItemDto.ProductId) 
        {
            var product = await ctx.Products.FindAsync(orderItemDto.ProductId);
            if (product == null)
            {
                return ServiceResult<OrderItem>.Failed("Product not found");
            }
            orderItem.ProductId = orderItemDto.ProductId;
        }

        if (orderItem.OrderId != orderItemDto.OrderId)
        {
            return ServiceResult<OrderItem>.Failed("You cannot change the order");
        }

        if (orderItem.Quantity != orderItemDto.Quantity && orderItemDto.Quantity >= 0)
        {
            orderItem.Quantity = orderItemDto.Quantity;
        }

        await ctx.SaveChangesAsync();
        
        return ServiceResult<OrderItem>.Ok(orderItem, "Order item updated successfully");
    }

    public async Task<ServiceResult<OrderItem>> FindById(int id)
    {
        var cacheKey = $"orderItem:{id}";
        if (cache.TryGetValue(cacheKey, out OrderItem? orderItem))
            if (orderItem != null)
                return ServiceResult<OrderItem>.Ok(orderItem, "Order item found");

        orderItem = await ctx.OrderItems.FindAsync(id);

        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, orderItem, cacheOption);
        
        return orderItem == null ? 
            ServiceResult<OrderItem>.Failed("Order item not found") : 
            ServiceResult<OrderItem>.Ok(orderItem, "Order item found");
    }

    public async Task<ServiceResult<List<OrderItem>>> FindByOrderId(int orderId)
    {
        var cacheKey = $"order:{orderId}:orderItem";
        if (cache.TryGetValue(cacheKey, out List<OrderItem>? orderItems))
            if (orderItems != null) 
                return ServiceResult<List<OrderItem>>.Ok(orderItems, "Order item of the selected order found");
        
        orderItems = await ctx.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Order)
                .ThenInclude(o => o.Items)
            .Include(oi => oi.Product)   
            .ToListAsync();
        
        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, orderItems, cacheOption);
        return orderItems.Count > 0 ? 
            ServiceResult<List<OrderItem>>.Ok(orderItems, "Order items of the selected order found") :
            ServiceResult<List<OrderItem>>.Failed("Order item of the selected order not found");
    }

    public async Task<ServiceResult<List<OrderItem>>> FindByProductId(int productId)
    {
        var cacheKey = $"product:{productId}:orderItem";
        if (cache.TryGetValue(cacheKey, out List<OrderItem>? orderItems)) 
            if (orderItems != null)
                return ServiceResult<List<OrderItem>>.Ok(orderItems, "Order items of the selected product found");
        
        orderItems = await ctx.OrderItems
            .Where(oi => oi.ProductId == productId)
            .Include(oi => oi.Product)
            .ToListAsync();

        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, orderItems, cacheOption);
        return orderItems.Count > 0 ? 
            ServiceResult<List<OrderItem>>.Ok(orderItems, "Order items of the selected product found") :
            ServiceResult<List<OrderItem>>.Failed("Order item of the selected product not found");
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var orderItem = await ctx.OrderItems.FindAsync(id);
        if (orderItem == null)
        {
            return ServiceResult.Failed("Order item not found");
        }
        
        cache.Remove($"orderItem:{id}");
        
        ctx.OrderItems.Remove(orderItem);
        await ctx.SaveChangesAsync();
        return ServiceResult.Ok("Order item deleted successfully");
    }
}