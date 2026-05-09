using Domain.Entity;

namespace Application.Dtos.Response;

public record InventoryResponse(int Id, int ProductId, int Stock, DateTime UpdatedAt)
{
    public static InventoryResponse FromEntity(Inventory inventory)
    {
        return new InventoryResponse(inventory.Id, inventory.ProductId, inventory.Stock, inventory.UpdatedAt);
    }
}
