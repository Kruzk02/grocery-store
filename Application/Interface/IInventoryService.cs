using Application.Common;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Queries;

using Domain.Entity;

namespace Application.Interface;

/// <summary>
/// Defines operations for managing inventories.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Asynchronously retrieve all inventory from the database.
    /// </summary>
    Task<PageResult<InventoryResponse>> FindAll(SearchInventoryQuery searchInventoryQuery);
    /// <summary>
    /// Asynchronously creates a new inventory in the database.
    /// </summary>
    /// <param name="inventoryDto">The <see cref="InventoryDto"/> that provide inventory data.</param>
    Task<InventoryResponse> Create(InventoryDto inventoryDto);
    /// <summary>
    /// Asynchronously updates an existing inventory in the database.
    /// </summary>
    /// <param name="id">The identifier of the inventory to update</param>
    /// <param name="inventoryDto">The <see cref="InventoryDto"/> that provides updated inventory data</param>
    Task<InventoryResponse> Update(int id, InventoryDto inventoryDto);
    /// <summary>
    /// Asynchronously retrieves an inventory by its identifier from the database.
    /// </summary>
    /// <param name="id">The identifier of the inventory to retrieve</param>
    Task<InventoryResponse> FindById(int id);
    /// <summary>
    /// Asynchronously deletes an inventory by its identifier from the database.
    /// </summary>
    /// <param name="id">The identifier of the inventory to delete.</param>
    Task<bool> Delete(int id);
}
