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
        var user = await userService.GetUser(User);
        return Ok(new { user.User });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var isCreated = await userService.CreateUser(dto);
        return isCreated ? Ok() : BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var loginResponse = await userService.Login(dto);
        return loginResponse.Success ? Ok(loginResponse) : Unauthorized(loginResponse);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
    {
        var isUpdated = await userService.UpdateUser(User, dto);
        return isUpdated ? Ok() : BadRequest();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var isDeleted = await userService.DeleteUser(request.Id);
        return isDeleted ? Ok() : BadRequest();
    }
}