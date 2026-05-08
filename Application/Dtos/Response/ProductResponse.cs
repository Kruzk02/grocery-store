using Domain.Entity;

namespace Application.Dtos.Response;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int CategoryId,
    int Quantity,
    string? ImagePath,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static ProductResponse FromEntity(Product product)
    {
        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.CategoryId,
            product.Quantity, product.ImagePath, product.CreatedAt, product.UpdatedAt);
    }
}
