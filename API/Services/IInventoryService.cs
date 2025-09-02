using API.Dtos;
using API.Entity;

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
    /// <returns>
    /// A task that represents the asynchronous operations. The task result contains
    /// a <see cref="ServiceResult{T}"/> with the list of inventory if successful;
    /// otherwise, error details
    /// </returns>
    Task<ServiceResult<List<Inventory>>> FindAll();
    /// <summary>
    /// Asynchronously creates a new inventory in the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which contains either the created inventory or error details.
    /// </remarks>
    /// <param name="inventoryDto">The <see cref="InventoryDto"/> that provide inventory data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The taks result contains
    /// a <see cref="ServiceResult{T}"/> with the created inventory if successful;
    /// otherwise, error details.
    /// </returns>
    Task<ServiceResult<Inventory>> Create(InventoryDto inventoryDto);
    /// <summary>
    /// Asynchronously updates an existing inventory in the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which indicates a success or contains error details.
    /// </remarks>
    /// <param name="id">The identifier of the inventory to update</param>
    /// <param name="inventoryDto">The <see cref="InventoryDto"/> that provides updated inventory data</param>
    /// <returns>
    /// A task represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult{T}"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult<Inventory>> Update(int id, InventoryDto inventoryDto);
    /// <summary>
    /// Asynchronously retrieves an inventory by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which contain either the inventory or error details.
    /// </remarks>
    /// <param name="id">The identifier of the inventory to retrieve</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult{T}"/> with the inventory if successful;
    /// otherwise, error details.
    /// </returns>
    Task<ServiceResult<Inventory>> FindById(int id);
    /// <summary>
    /// Asynchronously deletes an inventory by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which indicates success or contains error details. 
    /// </remarks>
    /// <param name="id">The identifier of the inventory to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult{T}"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult> Delete(int id);
}