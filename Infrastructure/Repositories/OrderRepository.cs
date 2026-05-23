using Application.Repositories;

using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories;

public class OrderRepository(ApplicationDbContext ctx) : IOrderRepository
{
    public async Task<Order> Add(Order order)
    {
        EntityEntry<Order> result = await ctx.Orders.AddAsync(order);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task Update(Order order)
    {
        ctx.Orders.Update(order);
        await ctx.SaveChangesAsync();
    }

    public async Task<Order?> FindById(int id)
    {
        return await ctx.Orders.FindAsync(id);
    }

    public async Task<List<Order>> FindByCustomerId(int customerId)
    {
        return await ctx.Orders.Where(o => o.CustomerId == customerId).ToListAsync();
    }

    public async Task Delete(Order order)
    {
        ctx.Orders.Remove(order);
        await ctx.SaveChangesAsync();
    }

}
