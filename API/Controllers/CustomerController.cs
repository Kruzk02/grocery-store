using Application.Common;
using Application.Dtos.Request;
using Application.Interface;
using Application.Queries;

using Domain.Entity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class CustomerController(ICustomerService customerService, IOrderService orderService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(List<Customer>), 200), ProducesResponseType(500)]
    public async Task<IActionResult> FindCustomer([FromQuery] SearchCustomerQuery searchCustomerQuery)
    {
        PageResult<Customer> pageResult = await customerService.SearchCustomers(searchCustomerQuery);
        return Ok(pageResult);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Customer), 201), ProducesResponseType(500)]
    public async Task<IActionResult> Create([FromBody] CustomerDto customerDto)
    {
        Customer result = await customerService.Create(customerDto);
        return CreatedAtAction(nameof(FindById), new { id = result.Id }, result);
    }

    [HttpGet("{id:int}/orders")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> GetOrders(int id)
    {
        return Ok(await orderService.FindByCustomerId(id));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerDto customerDto)
    {
        _ = await customerService.Update(id, customerDto);
        return NoContent();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindById(int id)
    {
        return Ok(await customerService.FindById(id));
    }

    [HttpGet("{name}/name")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindByName(string name)
    {
        return Ok(await customerService.FindByName(name));
    }

    [HttpGet("{email}/email")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindByEmail(string email)
    {
        return Ok(await customerService.FindByEmail(email));
    }

    [HttpGet("{phone}/phone")]
    [ProducesResponseType(typeof(Customer), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindByPhone(string phone)
    {
        return Ok(await customerService.FindByPhoneNumber(phone));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Customer), 204), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> DeleteById(int id)
    {
        _ = await customerService.DeleteById(id);
        return NoContent();
    }
}
