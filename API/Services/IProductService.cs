using API.Dtos;
using API.Entity;

namespace API.Services;

/// <summary>
/// Defines operations for managing products.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Asynchronously retrieves all products from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which contains either the list of products or error details.
    /// </remarks>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult{T}"/> with the list of products if successful;
    /// otherwise, error details.
    /// </returns>
    Task<ServiceResult<List<Product>>> FindAll();

    /// <summary>
    /// Asynchronously creates a new product in the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which contains either the created product or error details.
    /// </remarks>
    /// <param name="productDto">The <see cref="ProductDto"/> that provides product data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult{T}"/> with the created product if successful;
    /// otherwise, error details.
    /// </returns>
    Task<ServiceResult<Product>> Create(ProductDto productDto);

    /// <summary>
    /// Asynchronously updates an existing product in the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult"/> object,
    /// which indicates success or contains error details.
    /// </remarks>
    /// <param name="id">The identifier of the product to update.</param>
    /// <param name="productDto">The <see cref="ProductDto"/> that provides updated product data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult> Update(int id, ProductDto productDto);

    /// <summary>
    /// Asynchronously retrieves a product by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult{T}"/> object,
    /// which contains either the product or error details.
    /// </remarks>
    /// <param name="id">The identifier of the product to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult{T}"/> with the product if successful;
    /// otherwise, error details.
    /// </returns>
    Task<ServiceResult<Product>> FindById(int id);

    /// <summary>
    /// Asynchronously deletes a product by its identifier from the database.
    /// </summary>
    /// <remarks>
    /// The result is wrapped in a <see cref="ServiceResult"/> object,
    /// which indicates success or contains error details.
    /// </remarks>
    /// <param name="id">The identifier of the product to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="ServiceResult"/> indicating success or error details.
    /// </returns>
    Task<ServiceResult> DeleteById(int id);
}
