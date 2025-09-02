using API.Dtos;
using API.Entity;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]"), ApiController, Authorize]
public class InventoryController(IInventoryService service) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(typeof(Inventory), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> FindAll()
    {
        var serviceResult = await service.FindAll();
        return serviceResult.Success ? Ok(serviceResult) : BadRequest(serviceResult);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Inventory), 201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Create([FromBody] InventoryDto inventoryDto)
    {
        var serviceResult = await service.Create(inventoryDto);
        if (!serviceResult.Success)
        {
            return BadRequest(serviceResult);
        }
        
        return CreatedAtAction(nameof(FindById), new { id = serviceResult.Data!.Id }, serviceResult);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Update([FromBody] InventoryDto inventoryDto, int id)
    {
        var serviceResult = await service.Update(id, inventoryDto);
        return serviceResult.Success ? Ok(serviceResult) : BadRequest(serviceResult);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Inventory), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> FindById(int id)
    {
        var serviceResult = await service.FindById(id);
        return serviceResult.Success ? Ok(serviceResult) : BadRequest(serviceResult);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceResult = await service.Delete(id);
        return serviceResult.Success ? NoContent() : BadRequest(serviceResult);
    }
}