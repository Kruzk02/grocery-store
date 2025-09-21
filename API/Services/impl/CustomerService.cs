using API.Data;
using API.Dtos;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Services.impl;

public class CustomerService(ApplicationDbContext ctx) : ICustomerService
{
    public async Task<ServiceResult<List<Customer>>> FindAll()
    {
        var customers = await ctx.Customers.ToListAsync();
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
        var customer = await ctx.Customers.FindAsync(id);
        return customer != null ? 
            ServiceResult<Customer>.Ok(customer, "Customer found") :
            ServiceResult<Customer>.Failed("Customer not found");
    }

    public async Task<ServiceResult<Customer>> FindByEmail(string email)
    {
        var customer = await ctx.Customers.Where(c => c.Email == email).FirstOrDefaultAsync();
        
        return customer != null ? 
            ServiceResult<Customer>.Ok(customer, "Customer found") :
            ServiceResult<Customer>.Failed("Customer not found");
    }

    public async Task<ServiceResult<Customer>> FindByName(string name)
    {
        var customer = await ctx.Customers.Where(c => c.Name == name).FirstOrDefaultAsync();
        
        return customer != null ? 
            ServiceResult<Customer>.Ok(customer, "Customer found") :
            ServiceResult<Customer>.Failed("Customer not found");
    }

    public async Task<ServiceResult<Customer>> FindByPhoneNumber(string phoneNumber)
    {
        var customer = await ctx.Customers.Where(c => c.Phone == phoneNumber).FirstOrDefaultAsync();
        
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

       ctx.Customers.Remove(customer);
       await ctx.SaveChangesAsync();
       
       return ServiceResult.Ok("Customer deleted successfully");
    }
}