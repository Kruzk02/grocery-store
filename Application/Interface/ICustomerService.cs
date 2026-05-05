using Application.Common;
using Application.Dtos.Request;
using Application.Queries;

using Domain.Entity;

namespace Application.Interface;

public interface ICustomerService
{
    Task<PageResult<Customer>> SearchCustomers(SearchCustomerQuery searchCustomerQuery);
    Task<Customer> Create(CustomerDto customerDto);
    Task<string> Update(int id, CustomerDto customerDto);
    Task<Customer> FindById(int id);
    Task<Customer> FindByEmail(string email);
    Task<Customer> FindByName(string name);
    Task<Customer> FindByPhoneNumber(string phoneNumber);
    Task<string> DeleteById(int id);
}
