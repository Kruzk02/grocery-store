using API.Data;
using API.Entity;
using API.Services.impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Tests.Services;

[TestFixture]
public class CategoryServiceTest
{
    
    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
    
    [Test]
    public async Task GetAllCategories_ShouldReturn_ListOfCategory()
    {
        var ctx = GetInMemoryDbContext();
        var category = new Category { Id = 1, Name = "Fresh Produce", Description = "Fruits, vegetables, herbs" };
        ctx.Categories.Add(category);
        await ctx.SaveChangesAsync();
        
        var service = new CategoryService(ctx, new MemoryCache(new MemoryCacheOptions()));

        var result = service.FindAll();
        Assert.Multiple(() =>
        {
            Assert.That(result.Result.Success, Is.True);
            Assert.That(result.Result.Data, Is.Not.Null);
            Assert.That(result.Result.Data.Count, Is.EqualTo(1));
        });
    }
}