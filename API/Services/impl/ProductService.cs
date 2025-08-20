using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Services.impl;

/// <summary>
/// Provides operations for create, retrieve, update and delete product.
/// </summary>
/// <remarks>
/// This class interacts with database to performs CRUD operations related to produts.
/// </remarks>
/// <param name="ctx">the <see cref="ApplicationDbContext"/> used to access the database.</param>
public class ProductService(ApplicationDbContext ctx) : IProductService
{
    /// <inheritdoc />
    public async Task<ServiceResult<List<Product>>> FindAll()
    {
        var products = await ctx.Products.ToListAsync();
        return products.Count > 0 ? 
            ServiceResult<List<Product>>.Ok(products, "Products retrieve successfully") : 
            ServiceResult<List<Product>>.Failed("Failed to retrieve products"); 
    }

    ///  <inheritdoc/>
    public async Task<ServiceResult<Product>> Create(ProductDto productDto)
    {
        var category = await ctx.Categories.FindAsync(productDto.CategoryId);
        if (category == null)
        {
            return ServiceResult<Product>.Failed("Category not found");
        }
        
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            Quantity = productDto.Quantity,
            CategoryId = productDto.CategoryId,
            Category = category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var result = await ctx.Products.AddAsync(product);
        await ctx.SaveChangesAsync();
        
        return ServiceResult<Product>.Ok(result.Entity, "Product created successfully");
    }

    /// <inheritdoc/>
    public async Task<ServiceResult> Update(int id, ProductDto productDto)
    {
        var product = await ctx.Products.FindAsync(id);
        if (product == null)
        {
            return ServiceResult.Failed("Product not found");
        }

        if (!string.IsNullOrEmpty(productDto.Name) && productDto.Name != product.Name)
            product.Name = productDto.Name;

        if (!string.IsNullOrEmpty(productDto.Description) && productDto.Description != product.Description)
            product.Description = productDto.Description;

        if (productDto.Price >= 0 && productDto.Price != product.Price)
            product.Price = productDto.Price;

        if (productDto.Quantity >= 0 && productDto.Quantity != product.Quantity)
            product.Quantity = productDto.Quantity;

        if (productDto.CategoryId != product.CategoryId)
        {
            var category = await ctx.Categories.FindAsync(productDto.CategoryId);
            if (category == null)
            {
                return ServiceResult.Failed("Category not found");
            }
            
            product.CategoryId = category.Id;
            product.Category = category;
        }
        
        product.UpdatedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync();
        
        return ServiceResult.Ok("Product updated successfully");
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<Product>> FindById(int id)
    {
        var product = await ctx.Products.FindAsync(id);
        return product != null ?
            ServiceResult<Product>.Ok(product, "Product found") :
            ServiceResult<Product>.Failed("Product not found");
    }

    /// <inheritdoc/>
    public async Task<ServiceResult> DeleteById(int id)
    {
        var product = await ctx.Products.FindAsync(id);
        if (product == null)
        {
            return ServiceResult.Failed("Product not found");
        }
        
        ctx.Products.Remove(product);
        await ctx.SaveChangesAsync();
        
        return ServiceResult.Ok("Product deleted successfully");
    }
}