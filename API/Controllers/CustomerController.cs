using API.Dtos;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class CustomerController(ICustomerService customerService, IOrderService orderService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(List<Customer>), 200), ProducesResponseType(500)]
    public async Task<IActionResult> FindAll()
    {
        var serviceResult = await customerService.FindAll();
        return serviceResult.Success ? Ok(serviceResult.Data) : BadRequest(serviceResult);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Customer), 201), ProducesResponseType(500)]
    public async Task<IActionResult> Create([FromBody] CustomerDto customerDto)
    {
        var serviceResult = await customerService.Create(customerDto);
        if (!serviceResult.Success)
        {
            BadRequest(serviceResult);
        }
        
        return CreatedAtAction(nameof(FindById), new { id = serviceResult.Data!.Id }, serviceResult.Data);
    }

    [HttpGet("{id:int}/orders")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> GetOrders(int id)
    {
        var serviceResult = await orderService.FindByCustomerId(id);
        return serviceResult.Success ? Ok(serviceResult.Data) : BadRequest(serviceResult);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerDto customerDto)
    {
        var serviceResult = await customerService.Update(id, customerDto);
        return serviceResult.Success ? Ok() : BadRequest(serviceResult);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindById(int id)
    {
        var serviceResult = await customerService.FindById(id);
        return serviceResult.Success ? Ok(serviceResult.Data) : BadRequest(serviceResult);
    }
    
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindByName(string name)
    {
        var serviceResult = await customerService.FindByName(name);
        return serviceResult.Success ? Ok(serviceResult.Data) : BadRequest(serviceResult);
    }
    
    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindByEmail(string email)
    {
        var serviceResult = await customerService.FindByEmail(email);
        return serviceResult.Success ? Ok(serviceResult.Data) : BadRequest(serviceResult);
    }
    
    [HttpGet("phone/{phone}")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindByPhone(string phone)
    {
        var serviceResult = await customerService.FindByPhoneNumber(phone);
        return serviceResult.Success ? Ok(serviceResult.Data) : BadRequest(serviceResult);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Customer), 204), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> DeleteById(int id)
    {
        var serviceResult = await customerService.DeleteById(id);
        return serviceResult.Success ? NoContent() : NotFound(serviceResult);
    }
}