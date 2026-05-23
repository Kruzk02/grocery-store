using Application.Repositories;

using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext ctx) : ICategoryRepository
{

    public async Task<List<Category>> FindAll()
    {
        return await ctx.Categories.ToListAsync();
    }

    public async Task<Category?> FindById(int id)
    {
        return await ctx.Categories.FindAsync(id);
    }
}
