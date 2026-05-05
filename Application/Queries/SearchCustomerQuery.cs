namespace Application.Queries;

public record SearchCustomerQuery(string? Name, int Skip, int Take = 10);
