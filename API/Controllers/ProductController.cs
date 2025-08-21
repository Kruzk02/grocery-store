using API.Dtos;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Handles requests to products.
/// </summary>
/// <remarks>
/// This controller provided endpoints to Create, Retrieve, Update and Delete products.
/// </remarks>
/// <param name="productService"></param>
[ApiController, Route("[controller]"), Authorize]
public class ProductController(IProductService productService) : ControllerBase
{

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Product"/> object wrapped in a
    /// <see cref="ServiceResult"/>.
    /// </returns>
    /// <response code="200">Returns the list of categories.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> FindAll()
    {
        var serviceResult = await productService.FindAll();
        return serviceResult.Success ? Ok(serviceResult.Data) : BadRequest(serviceResult);
    }

    /// <summary>
    /// Create a new product.
    /// </summary>
    /// <param name="productDto">The product data to create.</param>
    /// <returns>
    /// The newly created <see cref="Product"/>.
    /// </returns>
    /// <response code="201">Returns the created product.</response>
    /// <response code="400">If the product data is invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Product), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Create([FromBody] ProductDto productDto)
    {
        var serviceResult = await productService.Create(productDto);
        if (!serviceResult.Success)
        {
            return BadRequest(serviceResult);
        }
        
        return CreatedAtAction(nameof(FindById), new { id = serviceResult.Data!.Id }, serviceResult.Data );
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="productDto">The update product data.</param>
    /// <returns>
    /// No content if the update was successful.
    /// </returns>
    /// <response code="204">Product was successfully updated.</response>
    /// <response code="400">If the product data is invalid.</response>
    /// <response code="404">If the product does not exist.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto productDto)
    {
        var serviceResult = await productService.Update(id, productDto);
        return serviceResult.Success ? Ok(serviceResult) : NotFound(serviceResult);
    }

    /// <summary>
    /// Retrieves a product by it ID.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
    /// <returns>
    /// The requested <see cref="Product"/>
    /// </returns>
    /// <response code="200">Returns the requested product.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> FindById(int id)
    {
        var serviceResult = await productService.FindById(id);
        return serviceResult.Success ? Ok(serviceResult) : NotFound(serviceResult);
    }

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>
    /// No content if the product was successfully deleted.
    /// </returns>
    /// <response code="204">Product was successfully deleted.</response>
    /// <response code="404">If the product is not found.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteById(int id)
    {
        var serviceResult = await productService.DeleteById(id);
        return serviceResult.Success ? NoContent() : NotFound(serviceResult);
    }
}