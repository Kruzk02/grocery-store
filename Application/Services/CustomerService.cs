using Application.Common;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interface;
using Application.Queries;
using Application.Repository;

using Domain.Entity;
using Domain.Exception;

using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class CustomerService(ICustomerRepository customerRepository, IMemoryCache cache) : ICustomerService
{
    public async Task<PageResult<CustomerResponse>> SearchCustomers(SearchCustomerQuery searchCustomerQuery)
    {
        PageResult<Customer> pageResult = await customerRepository.Search(searchCustomerQuery);
        return new PageResult<CustomerResponse>(Total:  pageResult.Total, Data: pageResult.Data.Select(CustomerResponse.FromEntity).ToList());
    }

    public async Task<CustomerResponse> Create(CustomerDto customerDto)
    {
        if (string.IsNullOrEmpty(customerDto.Name))
            throw new ValidationException(new Dictionary<string, string[]> { { "Name", ["Name is required"] } });
        if (string.IsNullOrEmpty(customerDto.Email))
            throw new ValidationException(new Dictionary<string, string[]> { { "Email", ["Email is required"] } });
        if (string.IsNullOrEmpty(customerDto.Phone))
            throw new ValidationException(new Dictionary<string, string[]> { { "Phone", ["Phone is required"] } });
        if (string.IsNullOrEmpty(customerDto.Address))
            throw new ValidationException(new Dictionary<string, string[]> { { "Address", ["Address is required"] } });

        var customer = new Customer
        {
            Name = customerDto.Name,
            Email = customerDto.Email,
            Phone = customerDto.Phone,
            Address = customerDto.Address
        };

        return CustomerResponse.FromEntity(await customerRepository.Add(customer));
    }

    public async Task<string> Update(int id, CustomerDto customerDto)
    {
        Customer? existingCustomer = await customerRepository.FindById(id);
        if (existingCustomer == null)
            throw new NotFoundException($"Customer with id: {id} not found");

        if (!string.IsNullOrEmpty(customerDto.Name) && customerDto.Name != existingCustomer.Name)
            existingCustomer.Name = customerDto.Name;
        if (!string.IsNullOrEmpty(customerDto.Email) && customerDto.Email != existingCustomer.Email)
            existingCustomer.Email = customerDto.Email;
        if (!string.IsNullOrEmpty(customerDto.Phone) && customerDto.Phone != existingCustomer.Phone)
            existingCustomer.Phone = customerDto.Phone;
        if (!string.IsNullOrEmpty(customerDto.Address) && customerDto.Address != existingCustomer.Address)
            existingCustomer.Address = customerDto.Address;

        await customerRepository.Update(existingCustomer);

        return "Customer updated successfully";
    }

    public async Task<CustomerResponse> FindById(int id)
    {
        var cacheKey = $"customer:{id}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
            Customer? customer = await customerRepository.FindById(id);
            return CustomerResponse.FromEntity(customer ??  throw new NotFoundException($"Customer with id: {id} not found"));
        }) ?? throw new InvalidOperationException();
    }

    public async Task<CustomerResponse> FindByEmail(string email)
    {
        var cacheKey = $"customer:email:{email}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
            Customer? customer = await customerRepository.FindByEmail(email);
            return CustomerResponse.FromEntity(customer ??  throw new NotFoundException($"Customer with email: {email} not found"));
        }) ?? throw new InvalidOperationException();
    }

    public async Task<CustomerResponse> FindByName(string name)
    {
        var cacheKey = $"customer:name:{name}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
            Customer? customer = await customerRepository.FindByName(name);
            return CustomerResponse.FromEntity(customer ??  throw new NotFoundException($"Customer with name: {name} not found"));
        }) ?? throw new InvalidOperationException();
    }

    public async Task<CustomerResponse> FindByPhoneNumber(string phoneNumber)
    {
        var cacheKey = $"customer:phone:{phoneNumber}";
        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(20));
            Customer? customer = await customerRepository.FindByPhoneNumber(phoneNumber);
            return CustomerResponse.FromEntity(customer ??  throw new NotFoundException($"Customer with phone number: {phoneNumber} not found"));
        }) ?? throw new InvalidOperationException();
    }

    public async Task<string> DeleteById(int id)
    {
        Customer? customer = await customerRepository.FindById(id);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with id: {id} not found");
        }

        cache.Remove($"customer:{id}");
        await customerRepository.Delete(customer);

        return "Customer deleted successfully";
    }
}
