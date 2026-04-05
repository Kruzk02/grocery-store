namespace Application.Queries;

public record SearchProductQuery(string? Name, int Skip, ProductSortBy? SortBy, bool Ascending, int Take = 10)
{ }

public enum ProductSortBy
{
    Name,
    Price,
    CreatedAt
}
