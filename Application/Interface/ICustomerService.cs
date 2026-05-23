using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
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
