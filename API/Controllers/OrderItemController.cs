using API.Dtos;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class OrderItemController(IOrderItemService orderItemService) : ControllerBase
{
    [HttpPost, 
     ProducesResponseType(typeof(OrderItem), 201), 
     ProducesResponseType(400), 
     ProducesResponseType(500),
    Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] OrderItemDto orderItemDto)
    {
        var serviceResult = await orderItemService.Create(orderItemDto);
        if (!serviceResult.Success)
        {
            return BadRequest(serviceResult);
        }
        return CreatedAtAction(nameof(FindById), new { id = serviceResult.Data!.Id }, serviceResult.Data);
    }

    [HttpPut("{id:int}"), 
     ProducesResponseType(typeof(OrderItem), 204), 
     ProducesResponseType(400),
     ProducesResponseType(404),
     ProducesResponseType(500),
    Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] OrderItemDto orderItemDto)
    {
        var serviceResult = await orderItemService.Update(id, orderItemDto);
        return serviceResult.Success ? NoContent() : BadRequest(serviceResult);
    }

    [HttpGet("{id:int}"),
     ProducesResponseType(typeof(OrderItem), 200),
     ProducesResponseType(400),
     ProducesResponseType(404),
     ProducesResponseType(500)]
    public async Task<IActionResult> FindById(int id)
    {
        var serviceResult = await orderItemService.FindById(id);
        return serviceResult.Success ? 
            Ok(serviceResult.Data) : 
            BadRequest(serviceResult);
    }

    [HttpDelete("{id:int}"),
     ProducesResponseType(204),
     ProducesResponseType(404),
     ProducesResponseType(500)]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceResult = await  orderItemService.Delete(id);
        return serviceResult.Success ? NoContent() : BadRequest(serviceResult);
    }
}