using Application.Common;
using Application.Queries;
using Application.Repository;

using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repository;

public class ProductRepository(ApplicationDbContext ctx) : IProductRepository
{
    public async Task<PageResult<Product>> Search(SearchProductQuery searchProductQuery)
    {
        IQueryable<Product> query = ctx.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searchProductQuery.Name))
        {
            query = query.Where(p => EF.Functions.Like(p.Name.ToLower(), $"%{searchProductQuery.Name.ToLower()}%"));
        }

        query = searchProductQuery.SortBy switch
        {
            ProductSortBy.Price => searchProductQuery.Ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
            ProductSortBy.Name => searchProductQuery.Ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            ProductSortBy.CreatedAt => searchProductQuery.Ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
            _ => query
        };

        var total = await query.CountAsync();
        List<Product> data = await query.Skip(searchProductQuery.Skip).Take(searchProductQuery.Take).ToListAsync();
        return new PageResult<Product>(total, data);
    }

    public async Task<Product> Add(Product product)
    {
        EntityEntry<Product> result = await ctx.Products.AddAsync(product);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task Update(Product product)
    {
        ctx.Products.Update(product);
        await ctx.SaveChangesAsync();
    }

    public async Task<Product?> FindById(int id)
    {
        return await ctx.Products.FindAsync(id);
    }

    public async Task Delete(Product product)
    {
        ctx.Products.Remove(product);
        await ctx.SaveChangesAsync();
    }
}
