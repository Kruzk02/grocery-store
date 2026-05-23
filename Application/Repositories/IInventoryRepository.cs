using Application.Common;
using Application.Queries;

using Domain.Entity;

namespace Application.Repositories;

public interface IInventoryRepository
{
    Task<PageResult<Inventory>> FindAll(SearchInventoryQuery searchInventoryQuery);
    Task<Inventory> Add(Inventory inventory);
    Task Update(Inventory inventory);
    Task<Inventory?> FindById(int id);
    Task<Inventory?> FindLessThanTenQuantity(CancellationToken stoppingToken);
    Task Delete(Inventory inventory);
}
