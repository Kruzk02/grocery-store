using API.Dtos;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class OrderController(IOrderService orderService, IOrderItemService itemService) : ControllerBase
{

    [HttpPost, 
     ProducesResponseType(typeof(Order), 201),
     ProducesResponseType(400),
     ProducesResponseType(500),
     Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] OrderDto orderDto)
    {
        var serviceResult = await orderService.Create(orderDto);
        if (!serviceResult.Success)
        {
            return BadRequest(serviceResult);
        }
        
        return CreatedAtAction(nameof(FindById), new { id = serviceResult.Data!.Id }, serviceResult.Data);
    }

    [HttpPut("{id:int}"), 
     ProducesResponseType(204), 
     ProducesResponseType(400),
     ProducesResponseType(404), 
     ProducesResponseType(500),
     Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] OrderDto orderDto)
    {
        var serviceResult = await orderService.Update(id, orderDto);
        return serviceResult.Success ? NoContent() : BadRequest(serviceResult);
    }

    [HttpGet("{id:int}"),
     ProducesResponseType(typeof(Order), 200),
     ProducesResponseType(404),
     ProducesResponseType(500)]
    public async Task<IActionResult> FindById(int id)
    {
        var serviceResult = await orderService.FindById(id);
        return serviceResult.Success ? 
            Ok(serviceResult.Data) : 
            BadRequest(serviceResult);
    }

    [HttpGet("{id:int}/ordersItem"),
     ProducesResponseType(typeof(Order), 200),
     ProducesResponseType(404),
     ProducesResponseType(500)]
    public async Task<IActionResult> FindOrderItemById(int id)
    {
        var serviceResult = await itemService.FindByOrderId(id);
        return serviceResult.Success ? 
            Ok(serviceResult.Data) : 
            BadRequest(serviceResult);
    }

    [HttpGet("{id:int}/invoice"),
     ProducesResponseType(typeof(Invoice), 200),
     ProducesResponseType(404),
     ProducesResponseType(500)]
    public async Task<IActionResult> FindInvoiceById(int id)
    {
        var serviceResult = await orderService.FindInvoiceByOrderId(id);
        return serviceResult.Success ?
            Ok(serviceResult.Data) : 
            BadRequest(serviceResult);
    }

    [HttpDelete("{id:int}"),
     ProducesResponseType(typeof(Order), 204),
     ProducesResponseType(404),
     ProducesResponseType(500),
     Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceResult = await orderService.Delete(id);
        return serviceResult.Success ?  NoContent() : BadRequest(serviceResult);
    }
}