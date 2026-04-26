using Application.Repository;

using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repository;

public class InventoryRepository(ApplicationDbContext ctx) : IInventoryRepository
{
    public async Task<(int total, List<Inventory> data)> FindAll(int? productId, int? stock, string? productName,
        int skip, int take)
    {
        IQueryable<Inventory> query = ctx.Inventories;

        if (productId.HasValue)
            query = query.Where(i => i.ProductId == productId.Value);

        if (stock.HasValue)
            query = query.Where(i => i.Stock >= stock.Value);

        if (!string.IsNullOrEmpty(productName))
            query = query.Where(i => i.Product.Name == productName);

        var total = await query.CountAsync();

        List<Inventory> data = await query
            .Include(i => i.Product)
            .ThenInclude(p => p.Category)
            .OrderByDescending(i => i.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (total, data);
    }

    public async Task<Inventory> Add(Inventory inventory)
    {
        EntityEntry<Inventory> result = await ctx.Inventories.AddAsync(inventory);
        _ = await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task Update(Inventory inventory)
    {
        _ = ctx.Inventories.Update(inventory);
        _ = await ctx.SaveChangesAsync();
    }

    public async Task<Inventory?> FindById(int id)
    {
        return await ctx.Inventories
            .Include(i => i.Product)
            .ThenInclude(p => p.Category)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Inventory?> FindLessThanTenQuantity(CancellationToken stoppingToken)
    {
        return await ctx.Inventories.Where(i => i.Stock <= 10).FirstOrDefaultAsync(stoppingToken);
    }

    public async Task Delete(Inventory inventory)
    {
        _ = ctx.Inventories.Remove(inventory);
        _ = await ctx.SaveChangesAsync();
    }
}
