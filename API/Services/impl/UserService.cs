using System.Security.Claims;
using API.Dtos;
using API.Entity;
using Microsoft.AspNetCore.Identity;

namespace API.Services.impl;

public class UserService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService, 
    IVerificationTokenService verificationTokenService,
    EmailService emailService
    ) : IUserService
{
    public async Task<ServiceResult> CreateUser(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };
        
        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded) return ServiceResult.Failed("User creation failed", result.Errors.Select(e => e.Description));
        var roleResult = await userManager.AddToRoleAsync(user, "User");
        
        var verificationToken = await verificationTokenService.GenerateVerificationToken(user);
        emailService.SendVerify(user.Email, verificationToken.Token);
        
        return roleResult.Succeeded ? 
            ServiceResult.Ok("User created successfully") : 
            ServiceResult.Failed("User created, but role assignment failed", result.Errors.Select(e => e.Description)); 
    }

    public async Task<ServiceResult> Login(LoginDto dto)
    {
        var user = await userManager.FindByNameAsync(dto.UserNameOrEmail) ?? await  userManager.FindByEmailAsync(dto.UserNameOrEmail);
        if (user == null) return ServiceResult.Failed("User not found");
        
        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return ServiceResult.Failed("Invalid password");

        var token = tokenService.CreateToken(user);
        return ServiceResult.Ok(token);
    }

    public async Task<ServiceResult> UpdateUser(ClaimsPrincipal user, UpdateUserDto dto)
    {
        var existingUser = await userManager.GetUserAsync(user);
        if (existingUser == null) return ServiceResult.Failed("User not found");

        if (!string.IsNullOrEmpty(dto.Email))
        {
            var emailResult = await userManager.SetEmailAsync(existingUser, dto.Email);
            if (!emailResult.Succeeded)  return ServiceResult.Failed("Failed to update email", emailResult.Errors.Select(e => e.Description));
        }

        if (!string.IsNullOrEmpty(dto.Username))
        {
            var usernameResult = await userManager.SetUserNameAsync(existingUser, dto.Username);
            if (!usernameResult.Succeeded) return ServiceResult.Failed("Failed to update username", usernameResult.Errors.Select(e => e.Description));
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            var removePassword = await userManager.RemovePasswordAsync(existingUser);
            if (!removePassword.Succeeded) return ServiceResult.Failed("Failed to remove password", removePassword.Errors.Select(e => e.Description));
            
            var addPassword = await userManager.AddPasswordAsync(existingUser, dto.Password);
            if (!addPassword.Succeeded) return ServiceResult.Failed("Failed to add password", addPassword.Errors.Select(e => e.Description));
        }
        
        return ServiceResult.Ok("User updated successfully");
    }

    public async Task<ServiceResult<ApplicationUser>> GetUser(ClaimsPrincipal user)
    {
        var existingUser = await userManager.GetUserAsync(user);
        if (existingUser == null) return ServiceResult<ApplicationUser>.Failed("User not found");
        
        return ServiceResult<ApplicationUser>.Ok(existingUser, "User retrieved successfully");
    }

    public async Task<ServiceResult> DeleteUser(string id)
    {
        var existingUser = await userManager.FindByIdAsync(id);
        if (existingUser == null) return ServiceResult.Failed("User not found");
        
        var result = await userManager.DeleteAsync(existingUser);
        return result.Succeeded ? ServiceResult.Ok("User deleted successfully") : ServiceResult.Failed("Failed to delete user", result.Errors.Select(e => e.Description));
    }

    public async Task<ServiceResult> VerifyAccount(string code)
    {
        var result = await verificationTokenService.VerifyToken(code);
        return result
            ? ServiceResult.Ok("User verify successfully")
            : ServiceResult.Failed("User verify failed");
    }
}