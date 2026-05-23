using Application.Common;
using Application.Dtos.Request;
using Application.DTOs.Response;
using Application.Queries;

using Domain.Entity;

namespace Application.Interface;

/// <summary>
/// Defines operations for managing products.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Asynchronously retrieves products with the name from the database.
    /// </summary>
    Task<PageResult<ProductResponse>> SearchProducts(SearchProductQuery searchProductQuery);

    /// <summary>
    /// Asynchronously creates a new product in the database.
    /// </summary>
    Task<ProductResponse> Create(ProductDto productDto);

    /// <summary>
    /// Asynchronously updates an existing product in the database.
    /// </summary>
    Task<ProductResponse> Update(int id, ProductDto productDto);

    /// <summary>
    /// Asynchronously retrieves a product by its identifier from the database.
    /// </summary>
    Task<ProductResponse> FindById(int id);

    /// <summary>
    /// Asynchronously deletes a product by its identifier from the database.
    /// </summary>
    Task<bool> DeleteById(int id);
}
