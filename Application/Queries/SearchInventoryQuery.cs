namespace Application.Queries;

public record SearchInventoryQuery(int? ProductId, int? Stock, string? ProductName, InventorySortBy? SortBy, bool Ascending, int Skip, int Take = 10);

public enum InventorySortBy
{
    ProductId,
    Stock,
    ProductName,
    UpdatedAt
}
