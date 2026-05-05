using Application.Common;
using Application.Dtos.Request;
using Application.Interface;
using Application.Queries;

using Domain.Entity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]"), ApiController, Authorize]
public class InventoryController(IInventoryService service) : ControllerBase
{
    [HttpGet, ProducesResponseType(typeof(Inventory), 200), ProducesResponseType(500)]
    public async Task<IActionResult> FindAll([FromQuery] SearchInventoryQuery searchInventoryQuery)
    {
        PageResult<Inventory> inventories = await service.FindAll(searchInventoryQuery);
        return Ok(inventories);
    }

    [HttpPost, ProducesResponseType(typeof(Inventory), 201), ProducesResponseType(401), ProducesResponseType(500)]
    public async Task<IActionResult> Create([FromBody] InventoryDto inventoryDto)
    {
        Inventory result = await service.Create(inventoryDto);
        return CreatedAtAction(nameof(FindById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}"), ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update([FromBody] InventoryDto inventoryDto, int id)
    {
        _ = await service.Update(id, inventoryDto);
        return NoContent();
    }

    [HttpGet("{id:int}"), ProducesResponseType(typeof(Inventory), 200), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> FindById(int id)
    {
        return Ok(await service.FindById(id));
    }

    [HttpDelete("{id:int}"), ProducesResponseType(204), ProducesResponseType(404), ProducesResponseType(500)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        _ = await service.Delete(id);
        return NoContent();
    }
}
