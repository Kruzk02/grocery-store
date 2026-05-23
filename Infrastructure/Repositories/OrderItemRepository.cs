
using Application.Repositories;

using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories;

public class OrderItemRepository(ApplicationDbContext ctx) : IOrderItemRepository
{
    public async Task<OrderItem> Add(OrderItem orderItem)
    {
        EntityEntry<OrderItem> result = await ctx.OrderItems.AddAsync(orderItem);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task Update(OrderItem orderItem)
    {
        ctx.OrderItems.Update(orderItem);
        await ctx.SaveChangesAsync();
    }

    public async Task<OrderItem?> FindById(int id)
    {
        return await ctx.OrderItems.FindAsync(id);
    }

    public async Task<List<OrderItem>> FindByOrderId(int orderId)
    {
        return await ctx.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Order)
                .ThenInclude(o => o.Items)
            .Include(oi => oi.Product)
            .ToListAsync();
    }

    public async Task<List<OrderItem>> FindByProductId(int productId)
    {
        return await ctx.OrderItems
            .Where(oi => oi.ProductId == productId)
            .Include(oi => oi.Product)
            .ToListAsync();
    }

    public async Task Delete(OrderItem orderItem)
    {
        ctx.OrderItems.Remove(orderItem);
        await ctx.SaveChangesAsync();
    }
}
