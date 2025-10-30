using Application.Dtos;
using Domain.Entity;

namespace Application.Services;

/// <summary>
/// Defines operations for managing products.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Asynchronously retrieves all products from the database.
    /// </summary>
    Task<List<Product>> FindAll();

    /// <summary>
    /// Asynchronously creates a new product in the database.
    /// </summary>
    Task<Product> Create(ProductDto productDto);

    /// <summary>
    /// Asynchronously updates an existing product in the database.
    /// </summary>
    Task<Product> Update(int id, ProductDto productDto);

    /// <summary>
    /// Asynchronously retrieves a product by its identifier from the database.
    /// </summary>
    Task<Product> FindById(int id);

    /// <summary>
    /// Asynchronously deletes a product by its identifier from the database.
    /// </summary>
    Task<bool> DeleteById(int id);
}
