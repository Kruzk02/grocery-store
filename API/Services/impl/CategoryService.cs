using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services.impl;

/// <summary>
/// Provides operations for retrieving categories.
/// </summary>
/// <remarks>
/// This class interacts with database to performs retrieve operations related to categories.
/// </remarks>
/// <param name="ctx">the <see cref="ApplicationDbContext"/> used to access the database.</param>
public class CategoryService(ApplicationDbContext ctx, IMemoryCache cache) : ICategoryService 
{
    /// <inheritdoc />
    public async Task<ServiceResult<List<Category>>> FindAll()
    {
        const string cacheKey = $"categories";
        if (cache.TryGetValue(cacheKey, out List<Category>? categories))
            if (categories != null)
                return ServiceResult<List<Category>>.Ok(categories, "Categories retrieved successfully");
                    
        categories = await ctx.Categories.ToListAsync();
        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, categories, cacheOption);
        return categories.Count > 0 ? 
            ServiceResult<List<Category>>.Ok(categories, "Categories retrieved successfully") : 
            ServiceResult<List<Category>>.Failed("Failed to find all categories");
    }
}