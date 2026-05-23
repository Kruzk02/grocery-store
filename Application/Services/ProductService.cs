using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Queries;
using Application.Repository;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

/// <summary>
/// Provides operations for create, retrieve, update and delete product.
/// </summary>
/// <remarks>
/// This class interacts with database to performs CRUD operations related to products.
/// </remarks>
public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IImageStorage imageStorage, IMemoryCache cache) : IProductService
{
    public async Task<PageResult<ProductResponse>> SearchProducts(SearchProductQuery searchProductQuery)
    {
        var cacheKey =
            $"products:{searchProductQuery.Name}:{searchProductQuery.Ascending}:{searchProductQuery.SortBy}:{searchProductQuery.Skip}:{searchProductQuery.Take}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
            PageResult<Product> pageResult = await productRepository.Search(searchProductQuery);
            return new PageResult<ProductResponse>(pageResult.Total, pageResult.Data.Select(ProductResponse.FromEntity).ToList());
        }) ?? throw new InvalidOperationException();
    }

    ///  <inheritdoc/>
    public async Task<ProductResponse> Create(ProductDto productDto)
    {
        Category? category = await categoryRepository.FindById(productDto.CategoryId);
        if (category == null)
        {
            throw new NotFoundException($"Category with id {productDto.CategoryId} not found");
        }

        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            Quantity = productDto.Quantity,
            CategoryId = productDto.CategoryId,
            Category = category,
            ImagePath = productDto.Filename,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return ProductResponse.FromEntity(await productRepository.Add(product));
    }

    /// <inheritdoc/>
    public async Task<ProductResponse> Update(int id, ProductDto productDto)
    {
        Product? product = await productRepository.FindById(id);
        if (product == null)
        {
            throw new NotFoundException($"Product with id {id} not found");
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
            Category? category = await categoryRepository.FindById(productDto.CategoryId);
            if (category == null)
            {
                throw new NotFoundException($"Category with id {productDto.CategoryId} not found");
            }

            product.CategoryId = category.Id;
            product.Category = category;
        }

        if (productDto.Filename != null && product.ImagePath != null)
        {
            await imageStorage.Delete(product.ImagePath);
            product.ImagePath = productDto.Filename;
        }

        product.UpdatedAt = DateTime.UtcNow;

        await productRepository.Update(product);

        return ProductResponse.FromEntity(product);
    }

    /// <inheritdoc/>
    public async Task<ProductResponse> FindById(int id)
    {
        var cacheKey = $"product:{id}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));

            Product? product = await productRepository.FindById(id);

            return ProductResponse.FromEntity(product ?? throw new NotFoundException($"Product with id {id} not found"));
        }) ?? throw new InvalidOperationException();
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteById(int id)
    {
        Product? product = await productRepository.FindById(id);
        if (product == null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }

        cache.Remove($"product:{id}");
        await productRepository.Delete(product);
        if (product.ImagePath != null)
            await imageStorage.Delete(product.ImagePath);
        return true;
    }
}
