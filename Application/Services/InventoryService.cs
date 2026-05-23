using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Queries;
using Application.Repositories;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

/// <summary>
/// Provides operations for create, retrieve, update and delete inventory.
/// </summary>
/// <remarks>
/// This class interacts with database to performs CRUD operations relate to inventory.
/// </remarks>
public class InventoryService(
    IInventoryRepository inventoryRepository,
    IProductRepository productRepository,
    IMemoryCache cache) : IInventoryService
{
    /// <inheritdoc />
    public async Task<PageResult<InventoryResponse>> FindAll(SearchInventoryQuery searchInventoryQuery)
    {
        var cacheKey =
            $"inventories:{searchInventoryQuery.ProductId}:{searchInventoryQuery.Stock}:{searchInventoryQuery.ProductName}:{searchInventoryQuery.Skip}:{searchInventoryQuery.Take}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));

            PageResult<Inventory> inventories = await inventoryRepository.FindAll(searchInventoryQuery);

            return new PageResult<InventoryResponse>(inventories.Total,
                inventories.Data.Select(InventoryResponse.FromEntity).ToList());
        }) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public async Task<InventoryResponse> Create(InventoryDto inventoryDto)
    {
        Product product = await productRepository.FindById(inventoryDto.ProductId) ??
                          throw new NotFoundException($"Product with id: {inventoryDto.ProductId} not found");
        var inventory = new Inventory
        {
            Product = product,
            ProductId = product.Id,
            Stock = inventoryDto.Stock,
            UpdatedAt = DateTime.UtcNow
        };

        return InventoryResponse.FromEntity(await inventoryRepository.Add(inventory));
    }

    /// <inheritdoc />
    public async Task<InventoryResponse> Update(int id, InventoryDto inventoryDto)
    {
        Inventory inventory = await inventoryRepository.FindById(id) ??
                              throw new NotFoundException($"Inventory with id: {id} not found");
        if (inventoryDto.Stock >= 0 && inventoryDto.Stock != inventory.Stock)
        {
            inventory.Stock = inventoryDto.Stock;
        }

        if (inventoryDto.ProductId != inventory.ProductId)
        {
            Product product = await productRepository.FindById(inventoryDto.ProductId) ??
                              throw new NotFoundException($"Product with id: {inventoryDto.ProductId} not found");
            inventory.Product = product;
            inventory.ProductId = product.Id;
        }

        inventory.UpdatedAt = DateTime.UtcNow;

        await inventoryRepository.Update(inventory);
        return InventoryResponse.FromEntity(inventory);
    }

    /// <inheritdoc />
    public async Task<InventoryResponse> FindById(int id)
    {
        var cacheKey = $"inventory:{id}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));

            Inventory? inventory = await inventoryRepository.FindById(id);

            return InventoryResponse.FromEntity(inventory ??
                                                throw new NotFoundException($"Inventory with id: {id} not found"));
        }) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc />
    public async Task<bool> Delete(int id)
    {
        Inventory inventory = await inventoryRepository.FindById(id) ??
                              throw new NotFoundException($"Inventory with id: {id} not found");
        cache.Remove($"inventory:{id}");
        await inventoryRepository.Delete(inventory);
        return true;
    }
}
