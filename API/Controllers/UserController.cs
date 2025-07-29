using API.Dtos;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController
    (
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        ITokenService tokenService
    ) : ControllerBase
{
    
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await userManager.GetUserAsync(User);
        return user != null ? Ok(new { user.UserName, user.Email }) : Unauthorized();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Username };
        
        var result = await userManager.CreateAsync(user, dto.Password);

        return !result.Succeeded
            ? BadRequest(result.Errors)
            : Ok(new { message = "User registered successfully", user = user.UserName });
    }

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