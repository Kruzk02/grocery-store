using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services.impl;

public class CustomerService(ApplicationDbContext ctx, IMemoryCache cache) : ICustomerService
{
    public async Task<ServiceResult<List<Customer>>> FindAll()
    {
        const string cacheKey = $"customers";
        if (cache.TryGetValue(cacheKey, out List<Customer>? customers))
            if (customers != null)
                return ServiceResult<List<Customer>>.Ok(customers, "Customers retrieve successfully");
        
        customers = await ctx.Customers.ToListAsync();
        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));

        cache.Set(cacheKey, customers, cacheOption);
        
        return customers.Count > 0 ?
            ServiceResult<List<Customer>>.Ok(customers, "Customers retrieve successfully") :
            ServiceResult<List<Customer>>.Failed("Failed to retrieve customers");
    }

    public async Task<ServiceResult<Customer>> Create(CustomerDto customerDto)
    {
        var customer = new Customer
        {
            Name = customerDto.Name,
            Email = customerDto.Email,
            Phone = customerDto.Phone,
            Address = customerDto.Address
        };
        
        var result = await ctx.AddAsync(customer);
        await ctx.SaveChangesAsync();
        
        return ServiceResult<Customer>.Ok(result.Entity, "Customer created successfully");
    }

    public async Task<ServiceResult> Update(int id, CustomerDto customerDto)
    {
        var existingCustomer = await ctx.Customers.FindAsync(id);
        if (existingCustomer == null)
        {
            return ServiceResult.Failed("Customer not found");
        }
        
        if (!string.IsNullOrEmpty(customerDto.Name) && customerDto.Name != existingCustomer.Name) 
            existingCustomer.Name = customerDto.Name;
        if (!string.IsNullOrEmpty(customerDto.Email) && customerDto.Email != existingCustomer.Email) 
            existingCustomer.Email = customerDto.Email;
        if (!string.IsNullOrEmpty(customerDto.Phone) && customerDto.Phone != existingCustomer.Phone) 
            existingCustomer.Phone = customerDto.Phone;
        if (!string.IsNullOrEmpty(customerDto.Address) && customerDto.Address != existingCustomer.Address)
            existingCustomer.Address = customerDto.Address;
        
        existingCustomer.UpdatedAt = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
        
        return ServiceResult.Ok("Customer updated successfully");
    }

    public async Task<ServiceResult<Customer>> FindById(int id)
    {
        var cacheKey = $"customer:{id}";
        
        if (cache.TryGetValue(cacheKey, out Customer? customer))
            if (customer != null)
                return ServiceResult<Customer>.Ok(customer, "Customer found");
        
        customer = await ctx.Customers.FindAsync(id);
        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, customer, cacheOption);
        return customer != null ? 
            ServiceResult<Customer>.Ok(customer, "Customer found") :
            ServiceResult<Customer>.Failed("Customer not found");
    }

    public async Task<ServiceResult<Customer>> FindByEmail(string email)
    {
        var cacheKey = $"customer:email:{email}";
        if (cache.TryGetValue(cacheKey, out Customer? customer))
            if (customer != null)
                return ServiceResult<Customer>.Ok(customer, "Customer found");
        
        customer = await ctx.Customers.Where(c => c.Email == email).FirstOrDefaultAsync();
        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, customer, cacheOption);
        
        return customer != null ? 
            ServiceResult<Customer>.Ok(customer, "Customer found") :
            ServiceResult<Customer>.Failed("Customer not found");
    }

    public async Task<ServiceResult<Customer>> FindByName(string name)
    {
        var cacheKey = $"customer:name:{name}";
        if (cache.TryGetValue(cacheKey, out Customer? customer))
            if (customer != null)
                return ServiceResult<Customer>.Ok(customer, "Customer found");
        
        customer = await ctx.Customers.Where(c => c.Name == name).FirstOrDefaultAsync();
        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, customer, cacheOption);
        return customer != null ? 
            ServiceResult<Customer>.Ok(customer, "Customer found") :
            ServiceResult<Customer>.Failed("Customer not found");
    }

    public async Task<ServiceResult<Customer>> FindByPhoneNumber(string phoneNumber)
    {
        var cacheKey = $"customer:phone:{phoneNumber}";
        if (cache.TryGetValue(cacheKey, out Customer? customer)) 
            if (customer != null)
                return ServiceResult<Customer>.Ok(customer, "Customer found");
        
        customer = await ctx.Customers.Where(c => c.Phone == phoneNumber).FirstOrDefaultAsync();
        var cacheOption = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
        
        cache.Set(cacheKey, customer, cacheOption);
        return customer != null ? 
            ServiceResult<Customer>.Ok(customer, "Customer found") :
            ServiceResult<Customer>.Failed("Customer not found");
    }

    public async Task<ServiceResult> DeleteById(int id)
    {
       var customer = await ctx.Customers.FindAsync(id);
       if (customer == null)
       {
           return ServiceResult.Failed("Customer not found");
       }

       cache.Remove($"customer:{id}");
       ctx.Customers.Remove(customer);
       await ctx.SaveChangesAsync();
       
       return ServiceResult.Ok("Customer deleted successfully");
    }
}