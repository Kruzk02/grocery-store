using Domain.Entity;

namespace Application.Dtos.Response;

public record CustomerResponse(
    int Id,
    string Name,
    string Email,
    string Phone,
    string Address,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static CustomerResponse FromEntity(Customer customer)
    {
        return new CustomerResponse(customer.Id, customer.Name, customer.Email, customer.Phone, customer.Address,
            customer.CreatedAt, customer.UpdatedAt);
    }
}
