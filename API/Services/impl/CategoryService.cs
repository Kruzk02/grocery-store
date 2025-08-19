using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Services.impl;

/// <summary>
/// Provides operations for retrieving categories.
/// </summary>
/// <remarks>
/// This class interacts with database to performs retrieve operations related to categories.
/// </remarks>
/// <param name="ctx">the <see cref="ApplicationDbContext"/> used to access the database.</param>
public class CategoryService(ApplicationDbContext ctx) : ICategoryService 
{
    /// <inheritdoc />
    public async Task<ServiceResult<List<Category>>> FindAll()
    {
        var categories = await ctx.Categories.ToListAsync();
        return categories.Count > 0 ? 
            ServiceResult<List<Category>>.Ok(categories, "Categories retrieved successfully") : 
            ServiceResult<List<Category>>.Failed("Failed to find all categories");
    }
}