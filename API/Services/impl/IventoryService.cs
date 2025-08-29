using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Services.impl;

public class InventoryService(ApplicationDbContext ctx) : IInventoryService
{
    public async Task<ServiceResult<List<Inventory>>> FindAll()
    {
        var inventories = await ctx.Inventories.ToListAsync();
        return inventories.Count > 0 ? 
            ServiceResult<List<Inventory>>.Ok(inventories, "Inventory retrieve successfully") :
            ServiceResult<List<Inventory>>.Failed("Failed to retrieve inventory");;
    }

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

    public async Task<ServiceResult<Inventory>> FindById(int id)
    {
        var inventory = await ctx.Inventories.FindAsync(id);
        return inventory != null
            ? ServiceResult<Inventory>.Ok(inventory, "Inventory retrieve successfully")
            : ServiceResult<Inventory>.Failed("Failed to retrieve inventory");
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var inventory = await ctx.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return ServiceResult.Failed("Inventory not found");
        }
        
        ctx.Inventories.Remove(inventory);
        await ctx.SaveChangesAsync();
        
        return ServiceResult.Ok("Inventory deleted successfully");
    }
}