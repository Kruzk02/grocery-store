using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
        IUserService userService
    ) : ControllerBase
{
    
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var result = await userService.GetUser(User);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await userService.CreateUser(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await userService.Login(dto);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
    {
        var result = await userService.UpdateUser(User, dto);
        if (result.Success) return Ok(result);

        if (result.Errors.Any(e => e.Contains("not found")))
            return NotFound(result);

        return BadRequest(result);
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var result = await userService.DeleteUser(request.Id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}