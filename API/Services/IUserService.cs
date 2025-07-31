using System.Security.Claims;
using API.Dtos;
using API.Entity;

namespace API.Services;

public interface IUserService
{
    Task<bool> CreateUser(RegisterDto dto);
    Task<LoginResponse> Login(LoginDto dto);
    Task<bool> UpdateUser(ClaimsPrincipal user, UpdateUserDto dto);
    Task<UserProfileResponse> GetUser(ClaimsPrincipal user);
    Task<bool> DeleteUser(string id);
}