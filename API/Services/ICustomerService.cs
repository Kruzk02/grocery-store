using API.Dtos;
using API.Entity;

namespace API.Services;

public interface ICustomerService
{
    Task<ServiceResult<List<Customer>>> FindAll();
    Task<ServiceResult<Customer>> Create(CustomerDto customerDto);
    Task<ServiceResult> Update(int id, CustomerDto customerDto);
    Task<ServiceResult<Customer>> FindById(int id);
    Task<ServiceResult<Customer>> FindByEmail(string email);
    Task<ServiceResult<Customer>> FindByName(string name);
    Task<ServiceResult<Customer>> FindByPhoneNumber(string phoneNumber);
    Task<ServiceResult> DeleteById(int id);
}