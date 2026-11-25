using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Interfaces;
using GestionProyectos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GestionProyectos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TareasController(ITareaService tareaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetAll(
        [FromQuery] EstadoTarea? estado,
        [FromQuery] PrioridadTarea? prioridad) =>
        Ok(await tareaService.GetAllAsync(estado, prioridad));

    [HttpGet("vencidas")]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetVencidas() =>
        Ok(await tareaService.GetVencidasAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TareaResponseDto>> GetById(int id) =>
        Ok(await tareaService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<TareaResponseDto>> Create(CreateTareaDto dto)
    {
        var creada = await tareaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TareaResponseDto>> Update(int id, UpdateTareaDto dto) =>
        Ok(await tareaService.UpdateAsync(id, dto));

    [HttpPatch("{id:int}/estado")]
    public async Task<ActionResult<TareaResponseDto>> CambiarEstado(int id, CambiarEstadoTareaDto dto) =>
        Ok(await tareaService.CambiarEstadoAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await tareaService.DeleteAsync(id);
        return NoContent();
    }
}