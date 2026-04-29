using Application.Common;
using Application.Dtos.Request;

using Domain.Entity;

namespace Application.Interface;

public interface ICustomerService
{
    Task<PageResult<Customer>> SearchCustomers(string? name, int skip, int take);
    Task<List<Customer>> FindAll();
    Task<Customer> Create(CustomerDto customerDto);
    Task<string> Update(int id, CustomerDto customerDto);
    Task<Customer> FindById(int id);
    Task<Customer> FindByEmail(string email);
    Task<Customer> FindByName(string name);
    Task<Customer> FindByPhoneNumber(string phoneNumber);
    Task<string> DeleteById(int id);
}
