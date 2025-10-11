using System.Security.Claims;
using API.Dtos;
using API.Entity;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public interface IUserService
{
    Task<string> CreateUser(RegisterDto dto);
    Task<string> Login(LoginDto dto);
    Task<string> UpdateUser(ClaimsPrincipal user, UpdateUserDto dto);
    Task<ApplicationUser> GetUser(ClaimsPrincipal user);
    Task<bool> DeleteUser(string id);
}