using System.Security.Claims;
using API.Dtos;
using API.Entity;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public class UserService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService, 
    IPasswordHasher<ApplicationUser> passwordHasher
    ) : IUserService
{
    public async Task<bool> CreateUser(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };
        
        var result = await userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded) await userManager.AddToRoleAsync(user, "User");

        return result.Succeeded; 
    }

    public async Task<LoginResponse> Login(LoginDto dto)
    {
        var user = await userManager.FindByNameAsync(dto.UserNameOrEmail) ?? await  userManager.FindByEmailAsync(dto.UserNameOrEmail);
        if (user == null) return new LoginResponse(false, string.Empty, "User not found");
        
        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return new LoginResponse(false, string.Empty, "Invalid password");

        var token = tokenService.CreateToken(user);
        return new LoginResponse(true, token, string.Empty);

    }

    public async Task<bool> UpdateUser(ClaimsPrincipal user, UpdateUserDto dto)
    {
        var existingUser = await userManager.GetUserAsync(user);
        if (existingUser == null) return false;
        
        existingUser.Email = dto.Email ?? existingUser.Email;
        existingUser.UserName = dto.Username ?? existingUser.UserName;
        existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, dto.Password) ?? existingUser.PasswordHash;
        
        var result = await userManager.UpdateAsync(existingUser);
        
        return result.Succeeded;
    }

    public async Task<UserProfileResponse> GetUser(ClaimsPrincipal user)
    {
        var existingUser = await userManager.GetUserAsync(user);
        return existingUser == null ? new UserProfileResponse(false, null!, "User not found") : new UserProfileResponse(true, existingUser, string.Empty);
    }

    public async Task<bool> DeleteUser(string id)
    {
        var existingUser = await userManager.FindByIdAsync(id);
        if (existingUser == null) return false;
        
        var result = await userManager.DeleteAsync(existingUser);
        return result.Succeeded;
    }
}