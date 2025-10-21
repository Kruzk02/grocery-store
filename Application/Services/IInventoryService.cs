using API.Dtos;
using Domain.Entity;

namespace API.Services;

/// <summary>
/// Defines operations for managing inventories.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Asynchronously retrieve all inventory from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which contains either the list of inventory or error details
    /// </remarks>
    Task<List<Inventory>> FindAll();
    /// <summary>
    /// Asynchronously creates a new inventory in the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which contains either the created inventory or error details.
    /// </remarks>
    /// <param name="inventoryDto">The <see cref="InventoryDto"/> that provide inventory data.</param>
    Task<Inventory> Create(InventoryDto inventoryDto);
    /// <summary>
    /// Asynchronously updates an existing inventory in the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which indicates a success or contains error details.
    /// </remarks>
    /// <param name="id">The identifier of the inventory to update</param>
    /// <param name="inventoryDto">The <see cref="InventoryDto"/> that provides updated inventory data</param>
    Task<Inventory> Update(int id, InventoryDto inventoryDto);
    /// <summary>
    /// Asynchronously retrieves an inventory by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which contain either the inventory or error details.
    /// </remarks>
    /// <param name="id">The identifier of the inventory to retrieve</param>
    Task<Inventory> FindById(int id);
    /// <summary>
    /// Asynchronously deletes an inventory by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which indicates success or contains error details. 
    /// </remarks>
    /// <param name="id">The identifier of the inventory to delete.</param>
    Task<string> Delete(int id);
}