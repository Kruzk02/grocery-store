using Application.Common;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Queries;

using Domain.Entity;

namespace Application.Interface;

public interface ICustomerService
{
    Task<PageResult<CustomerResponse>> SearchCustomers(SearchCustomerQuery searchCustomerQuery);
    Task<CustomerResponse> Create(CustomerDto customerDto);
    Task<string> Update(int id, CustomerDto customerDto);
    Task<CustomerResponse> FindById(int id);
    Task<CustomerResponse> FindByEmail(string email);
    Task<CustomerResponse> FindByName(string name);
    Task<CustomerResponse> FindByPhoneNumber(string phoneNumber);
    Task<string> DeleteById(int id);
}
