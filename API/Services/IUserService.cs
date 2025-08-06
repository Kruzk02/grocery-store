using System.Security.Claims;
using API.Dtos;
using API.Entity;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public interface IUserService
{
    Task<ServiceResult> CreateUser(RegisterDto dto);
    Task<ServiceResult> Login(LoginDto dto);
    Task<ServiceResult> UpdateUser(ClaimsPrincipal user, UpdateUserDto dto);
    Task<ServiceResult<ApplicationUser>> GetUser(ClaimsPrincipal user);
    Task<ServiceResult> DeleteUser(string id);
    Task<ServiceResult> VerifyAccount(string code);
}