using System.Security.Claims;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    
    [ProducesResponseType(typeof(Notification), 204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        var serviceResult = await notificationService.DeleteById(id);
        return serviceResult.Success ? NoContent() : NotFound();
    }
    
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Notification), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var serviceResult = await notificationService.MarkAsRead(id);
        return serviceResult.Success ? Ok() : NotFound();
    }
    
    [HttpPut("all-as-read")]
    [ProducesResponseType(typeof(Notification), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return BadRequest();
        }
        var serviceResult = await notificationService.MarkAllAsRead(userId);
        return serviceResult.Success ? Ok() : NotFound();
    }
}