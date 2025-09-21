using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Services.impl;

public class OrderItemService(ApplicationDbContext ctx) : IOrderItemService
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
        var orderItem = await ctx.OrderItems.FindAsync(id);
        return orderItem == null ? 
            ServiceResult<OrderItem>.Failed("Order item not found") : 
            ServiceResult<OrderItem>.Ok(orderItem, "Order item found");
    }

    public async Task<ServiceResult<List<OrderItem>>> FindByOrderId(int orderId)
    {
        var orderItems = await ctx.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
        return orderItems.Count > 0 ? 
            ServiceResult<List<OrderItem>>.Ok(orderItems, "Order items found") :
            ServiceResult<List<OrderItem>>.Failed("Order item not found");
    }

    public async Task<ServiceResult<List<OrderItem>>> FindByProductId(int productId)
    {
        var orderItems = await ctx.OrderItems.Where(oi => oi.ProductId == productId).ToListAsync();
        return orderItems.Count > 0 ? 
            ServiceResult<List<OrderItem>>.Ok(orderItems, "Order items found") :
            ServiceResult<List<OrderItem>>.Failed("Order item not found");
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var orderItem = await ctx.OrderItems.FindAsync(id);
        if (orderItem == null)
        {
            return ServiceResult.Failed("Order item not found");
        }

        ctx.OrderItems.Remove(orderItem);
        await ctx.SaveChangesAsync();
        return ServiceResult.Ok("Order item deleted successfully");
    }
}