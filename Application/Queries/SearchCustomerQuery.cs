namespace Application.Queries;

public record SearchCustomerQuery(string? Name, CustomerSortBy? SortBy, bool Ascending, int Skip, int Take = 10);

public enum CustomerSortBy
{
    Name,
    Email,
    Phone,
    Address,
    CreatedAt
}
