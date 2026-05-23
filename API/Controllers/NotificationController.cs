using System.Security.Claims;

using Application.DTOs.Response;
using Application.Interface;

using Domain.Entity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class NotificationController(INotificationService notificationService) : ControllerBase
{

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Notification), 204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        _ = await notificationService.DeleteById(id);
        return NoContent();
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Notification), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        NotificationResponse result = await notificationService.MarkAsRead(id);
        return Ok(result);
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
        List<NotificationResponse> result = await notificationService.MarkAllAsRead(userId);
        return Ok(result);
    }
}
