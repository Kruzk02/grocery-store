using API.Dtos;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, 
        ITokenService tokenService
    ) : ControllerBase
{
    
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var roles = await userManager.GetRolesAsync(user);
        return Ok(new { user.UserName, user.Email, roles });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };
        
        var result = await userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded) await userManager.AddToRoleAsync(user, "User");
        
        return !result.Succeeded
            ? BadRequest(result.Errors)
            : Ok(new { message = "User registered successfully" });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await userManager.FindByNameAsync(dto.UserNameOrEmail)
                   ?? await userManager.FindByEmailAsync(dto.UserNameOrEmail);
        if (user == null) Unauthorized();

        var result = await signInManager.CheckPasswordSignInAsync(user!, dto.Password, false);
        
        if (!result.Succeeded) Unauthorized();

        var token = tokenService.CreateToken(user!);

        return Ok(new { token });
    }
}