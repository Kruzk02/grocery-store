using API.Dtos;
using API.Entity;

namespace API.Services;

public interface IInventoryService
{
    Task<ServiceResult<List<Inventory>>> FindAll();
    Task<ServiceResult<Inventory>> Create(InventoryDto inventoryDto);
    Task<ServiceResult<Inventory>> Update(int id, InventoryDto inventoryDto);
    Task<ServiceResult<Inventory>> FindById(int id);
    Task<ServiceResult> Delete(int id);
}