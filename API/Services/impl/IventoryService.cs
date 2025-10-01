using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services.impl;

/// <summary>
/// Provides operations for create, retrieve, update and delete inventory.
/// </summary>
/// <remarks>
/// This class interacts with database to performs CRUD operations relate to inventory.
/// </remarks>
/// <param name="ctx">The <see cref="ApplicationDbContext"/> used to access the database</param>
public class InventoryService(ApplicationDbContext ctx, IMemoryCache cache) : IInventoryService
{
    /// <inheritdoc />
    public async Task<ServiceResult<List<Inventory>>> FindAll()
    {
        const string cacheKey = $"inventories";
        if (cache.TryGetValue(cacheKey, out List<Inventory>? inventories))
            if (inventories != null) 
                return ServiceResult<List<Inventory>>.Ok(inventories);
        
        inventories = await ctx.Inventories.ToListAsync();

        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, inventories, cacheOption);
        
        return inventories.Count > 0 ? 
            ServiceResult<List<Inventory>>.Ok(inventories, "Inventory retrieve successfully") :
            ServiceResult<List<Inventory>>.Failed("Failed to retrieve inventory");;
    }

    /// <inheritdoc />
    public async Task<ServiceResult<Inventory>> Create(InventoryDto inventoryDto)
    {
        var product = await ctx.Products.FindAsync(inventoryDto.ProductId);
        if (product == null)
        {
            return ServiceResult<Inventory>.Failed("Product not found");
        }

        var inventory = new Inventory
        {
            Product = product,
            ProductId = product.Id,
            Quantity = inventoryDto.Quantity,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await ctx.Inventories.AddAsync(inventory);
        await ctx.SaveChangesAsync();
        
        return ServiceResult<Inventory>.Ok(result.Entity, "Inventory added successfully");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<Inventory>> Update(int id, InventoryDto inventoryDto)
    {
        var inventory = await ctx.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return ServiceResult<Inventory>.Failed("Inventory not found");
        }

        if (inventoryDto.Quantity >= 0 && inventoryDto.Quantity != inventory.Quantity)
        {
            inventory.Quantity = inventoryDto.Quantity;
        }

        if (inventoryDto.ProductId != inventory.ProductId)
        {
            var product = await ctx.Products.FindAsync(inventoryDto.ProductId);
            if (product == null)
            {
                return ServiceResult<Inventory>.Failed("Product not found");
            }

            inventory.Product = product;
            inventory.ProductId = product.Id;
        }
        
        inventory.UpdatedAt = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
        
        return ServiceResult<Inventory>.Ok(inventory, "Inventory updated successfully");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<Inventory>> FindById(int id)
    {
        var cacheKey = $"inventory:{id}";
        if (cache.TryGetValue(cacheKey, out Inventory? inventory)) 
            if (inventory != null)
                return ServiceResult<Inventory>.Ok(inventory, "Inventory retrieved successfully");
        
        inventory = await ctx.Inventories.FindAsync(id);

        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, inventory, cacheOption);
        
        return inventory != null
            ? ServiceResult<Inventory>.Ok(inventory, "Inventory retrieve successfully")
            : ServiceResult<Inventory>.Failed("Inventory not found");
    }

    /// <inheritdoc />
    public async Task<ServiceResult> Delete(int id)
    {
        var inventory = await ctx.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return ServiceResult.Failed("Inventory not found");
        }
        
        cache.Remove($"inventory:{id}");
        ctx.Inventories.Remove(inventory);
        await ctx.SaveChangesAsync();
        
        return ServiceResult.Ok("Inventory deleted successfully");
    }
}