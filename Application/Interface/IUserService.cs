
using System.Security.Claims;

using Application.DTOs.Request;
using Application.DTOs.Response;

using Domain.Entity;

namespace Application.Interface;

public interface IUserService
{
    Task<UserResponse> CreateUser(RegisterDto dto);
    Task<AuthResponse> Login(LoginDto dto);
    Task<UserResponse> GetUser(string usernameOrEmail);
    Task<AuthResponse> RefreshToken(string refreshToken);
    Task<bool> UpdateUser(string id, UpdateUserDto dto);
    Task Logout(ClaimsPrincipal claims);
    Task<bool> DeleteUser(string id);
}
