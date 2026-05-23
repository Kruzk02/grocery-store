
using Application.Common;
using Application.Queries;

using Domain.Entity;

namespace Application.Repositories;

public interface ICustomerRepository
{
    Task<PageResult<Customer>> Search(SearchCustomerQuery searchCustomerQuery);
    Task<Customer> Add(Customer customer);
    Task Update(Customer customer);
    Task<Customer?> FindById(int id);
    Task<Customer?> FindByEmail(string email);
    Task<Customer?> FindByName(string name);
    Task<Customer?> FindByPhoneNumber(string phoneNumber);
    Task Delete(Customer customer);
}
