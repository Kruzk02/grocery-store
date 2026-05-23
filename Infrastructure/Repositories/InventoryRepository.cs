using Application.Common;
using Application.Queries;
using Application.Repositories;

using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories;

public class InventoryRepository(ApplicationDbContext ctx) : IInventoryRepository
{
    public async Task<PageResult<Inventory>> FindAll(SearchInventoryQuery searchInventoryQuery)
    {
        IQueryable<Inventory> query = ctx.Inventories;

        if (searchInventoryQuery.ProductId.HasValue)
            query = query.Where(i => i.ProductId == searchInventoryQuery.ProductId.Value);

        if (searchInventoryQuery.Stock.HasValue)
            query = query.Where(i => i.Stock >= searchInventoryQuery.Stock.Value);

        if (!string.IsNullOrEmpty(searchInventoryQuery.ProductName))
            query = query.Where(i => i.Product.Name == searchInventoryQuery.ProductName);

        query = searchInventoryQuery.SortBy switch
        {
            InventorySortBy.ProductId => searchInventoryQuery.Ascending
                ? query.OrderBy(i => i.ProductId)
                : query.OrderByDescending(i => i.ProductId),
            InventorySortBy.ProductName => searchInventoryQuery.Ascending
                ? query.OrderBy(i => i.Product.Name)
                : query.OrderByDescending(i => i.Product.Name),
            InventorySortBy.Stock => searchInventoryQuery.Ascending
                ? query.OrderBy(i => i.Stock)
                : query.OrderByDescending(i => i.Stock),
            InventorySortBy.UpdatedAt => searchInventoryQuery.Ascending
                ? query.OrderBy(i => i.UpdatedAt)
                : query.OrderByDescending(i => i.UpdatedAt),
            _ => query
        };

        var total = await query.CountAsync();

        List<Inventory> data = await query
            .Include(i => i.Product)
            .ThenInclude(p => p.Category)
            .OrderByDescending(i => i.Id)
            .Skip(searchInventoryQuery.Skip)
            .Take(searchInventoryQuery.Take)
            .ToListAsync();

        return new PageResult<Inventory>(total, data);
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
