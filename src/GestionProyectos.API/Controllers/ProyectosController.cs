using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionProyectos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProyectosController(IProyectoService proyectoService, ITareaService tareaService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProyectoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProyectoResponseDto>>> GetAll() =>
        Ok(await proyectoService.GetAllAsync());

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProyectoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProyectoResponseDto>> GetById(int id) =>
        Ok(await proyectoService.GetByIdAsync(id));

    [HttpGet("{id:int}/tareas")]
    [ProducesResponseType(typeof(IEnumerable<TareaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetTareasByProyecto(int id) =>
        Ok(await tareaService.GetByProyectoIdAsync(id));

    [HttpPost]
    [ProducesResponseType(typeof(ProyectoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProyectoResponseDto>> Create(CreateProyectoDto dto)
    {
        var creado = await proyectoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProyectoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProyectoResponseDto>> Update(int id, UpdateProyectoDto dto) =>
        Ok(await proyectoService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        await proyectoService.DeleteAsync(id);
        return NoContent();
    }
}
