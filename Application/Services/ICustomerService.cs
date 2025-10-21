using API.Dtos;
using Domain.Entity;

namespace API.Services;

public interface ICustomerService
{
    Task<List<Customer>> FindAll();
    Task<Customer> Create(CustomerDto customerDto);
    Task<string> Update(int id, CustomerDto customerDto);
    Task<Customer> FindById(int id);
    Task<Customer> FindByEmail(string email);
    Task<Customer> FindByName(string name);
    Task<Customer> FindByPhoneNumber(string phoneNumber);
    Task<string> DeleteById(int id);
}