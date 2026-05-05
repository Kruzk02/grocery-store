using Application.Common;
using Application.Queries;
using Application.Repository;

using Domain.Entity;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repository;

public class CustomerRepository(ApplicationDbContext ctx) : ICustomerRepository
{
    public async Task<PageResult<Customer>> Search(SearchCustomerQuery searchCustomerQuery)
    {
        IQueryable<Customer> query = ctx.Customers.AsQueryable();
        if (!string.IsNullOrEmpty(searchCustomerQuery.Name))
        {
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, $"%{searchCustomerQuery.Name}%"));
        }

        query = searchCustomerQuery.SortBy switch
        {
            CustomerSortBy.Name => searchCustomerQuery.Ascending
                ? query.OrderBy(c => c.Name)
                : query.OrderByDescending(c => c.Name),
            CustomerSortBy.Email => searchCustomerQuery.Ascending
                ? query.OrderBy(c => c.Email)
                : query.OrderByDescending(c => c.Email),
            CustomerSortBy.Phone => searchCustomerQuery.Ascending
                ? query.OrderBy(c => c.Phone)
                : query.OrderByDescending(c => c.Phone),
            CustomerSortBy.Address => searchCustomerQuery.Ascending
                ? query.OrderBy(c => c.Address)
                : query.OrderByDescending(c => c.Address),
            CustomerSortBy.CreatedAt => searchCustomerQuery.Ascending
                ? query.OrderBy(c => c.CreatedAt)
                : query.OrderByDescending(c => c.CreatedAt),
            _ => query
        };

        var total = await query.CountAsync();
        List<Customer> data = await query.Skip(searchCustomerQuery.Skip).Take(searchCustomerQuery.Take).ToListAsync();

        return new PageResult<Customer>(total, data);
    }

    public async Task<Customer> Add(Customer customer)
    {
        EntityEntry<Customer> result = await ctx.AddAsync(customer);
        await ctx.SaveChangesAsync();
        return result.Entity;
    }

    public async Task Update(Customer customer)
    {
        ctx.Customers.Update(customer);
        await ctx.SaveChangesAsync();
    }

    public async Task<Customer?> FindById(int id)
    {
        return await ctx.Customers.FindAsync(id);
    }

    public async Task<Customer?> FindByEmail(string email)
    {
        return await ctx.Customers.Where(c => c.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Customer?> FindByName(string name)
    {
        return await ctx.Customers.Where(c => c.Name == name).FirstOrDefaultAsync();
    }

    public async Task<Customer?> FindByPhoneNumber(string phoneNumber)
    {
        return await ctx.Customers.Where(c => c.Phone == phoneNumber).FirstOrDefaultAsync();
    }

    public async Task Delete(Customer customer)
    {
        ctx.Customers.Remove(customer);
        await ctx.SaveChangesAsync();
    }
}
