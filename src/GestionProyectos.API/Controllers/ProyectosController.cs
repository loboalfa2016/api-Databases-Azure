using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionProyectos.API.Controllers;

[ApiController]
[Route("api/v1/proyectos")]
public class ProyectosController(IProyectoService proyectoService, ITareaService tareaService) : ControllerBase
{
    /// <summary>Listar proyectos - Obtiene todos los proyectos con su porcentaje de avance.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProyectoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProyectoResponseDto>>> GetAll() =>
        Ok(await proyectoService.GetAllAsync());

    /// <summary>Obtener proyecto por ID - Devuelve el detalle de un proyecto específico.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProyectoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProyectoResponseDto>> GetById(int id) =>
        Ok(await proyectoService.GetByIdAsync(id));

    /// <summary>Listar tareas del proyecto - Devuelve todas las tareas asociadas a un proyecto.</summary>
    [HttpGet("{id:int}/tareas")]
    [ProducesResponseType(typeof(IEnumerable<TareaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetTareasByProyecto(int id) =>
        Ok(await tareaService.GetByProyectoIdAsync(id));

    /// <summary>Crear proyecto - Registra un nuevo proyecto en la base de datos.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProyectoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProyectoResponseDto>> Create(CreateProyectoDto dto)
    {
        var creado = await proyectoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    /// <summary>Actualizar proyecto - Modifica los datos de un proyecto existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProyectoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProyectoResponseDto>> Update(int id, UpdateProyectoDto dto) =>
        Ok(await proyectoService.UpdateAsync(id, dto));

    /// <summary>Eliminar proyecto - Borra un proyecto solo si no tiene tareas pendientes.</summary>
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
