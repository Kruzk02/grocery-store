using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services.impl;

public class OrderService(ApplicationDbContext ctx, IMemoryCache cache) : IOrderService
{
    public async Task<ServiceResult<Order>> Create(OrderDto orderDto)
    {
        var customer = await ctx.Customers.FindAsync(orderDto.CustomerId);
        if (customer == null)
        {
            return ServiceResult<Order>.Failed("Customer not found");
        }

        var order = new Order
        {
            CustomerId = orderDto.CustomerId,
            customer = customer
        };
        
        var result = await ctx.Orders.AddAsync(order);
        await ctx.SaveChangesAsync();
        
        return ServiceResult<Order>.Ok(result.Entity, "Order created successfully");
    }

    public async Task<ServiceResult> Update(int id, OrderDto orderDto)
    {
        var order = await ctx.Orders.FindAsync(id);
        if (order == null)
        {
            return ServiceResult.Failed("Order not found");
        }

        if (orderDto.CustomerId != order.CustomerId && orderDto.CustomerId != 0)
        {
            var customer = await ctx.Customers.FindAsync(orderDto.CustomerId);
            if (customer == null)
            {
                return ServiceResult.Failed("Customer not found");
            }
            
            order.CustomerId = orderDto.CustomerId;
            order.customer = customer;
        }

        await ctx.SaveChangesAsync();
        
        return ServiceResult.Ok("Order updated successfully");
    }

    public async Task<ServiceResult<Order>> FindById(int id)
    {
        var cacheKey = $"order:{id}";
        if (cache.TryGetValue(cacheKey, out Order? order))
            if (order != null)
                return ServiceResult<Order>.Ok(order, "Order found");
        order = await ctx.Orders.FindAsync(id);
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        cache.Set(cacheKey, order, cacheOptions);

        return order != null
            ? ServiceResult<Order>.Ok(order, "Order found")
            : ServiceResult<Order>.Failed("Order not found");
    }

    public async Task<ServiceResult<List<Order>>> FindByCustomerId(int customerId)
    {
        var cacheKey = $"customer:{customerId}:orders";
        if (cache.TryGetValue(cacheKey, out List<Order>? orders)) 
            if (orders != null)
                return ServiceResult<List<Order>>.Ok(orders, "Orders found");
        
        orders = await ctx.Orders.Where(o => o.CustomerId == customerId).ToListAsync();
        
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        cache.Set(cacheKey, orders, cacheOptions);
        
        return orders.Count > 0 ? 
            ServiceResult<List<Order>>.Ok(orders, "Orders found") :
            ServiceResult<List<Order>>.Failed("Order not found");
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var order = await ctx.Orders.FindAsync(id);
        if (order == null)
        {
            return ServiceResult.Failed("Order not found");
        }
        
        cache.Remove($"order:{id}");
        ctx.Orders.Remove(order);
        await ctx.SaveChangesAsync();
        return ServiceResult.Ok("Order deleted successfully");
    }
}