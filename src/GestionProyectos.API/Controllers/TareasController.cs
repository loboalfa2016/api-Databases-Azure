using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Interfaces;
using GestionProyectos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GestionProyectos.API.Controllers;

[ApiController]
[Route("api/v1/tareas")]
public class TareasController(ITareaService tareaService) : ControllerBase
{
    /// <summary>Listar tareas - Obtiene todas las tareas con filtros opcionales por estado y prioridad.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TareaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetAll(
        [FromQuery] EstadoTarea? estado,
        [FromQuery] PrioridadTarea? prioridad) =>
        Ok(await tareaService.GetAllAsync(estado, prioridad));

    /// <summary>Listar tareas vencidas - Obtiene tareas cuya fecha límite expiró y siguen sin completar.</summary>
    [HttpGet("vencidas")]
    [ProducesResponseType(typeof(IEnumerable<TareaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetVencidas() =>
        Ok(await tareaService.GetVencidasAsync());

    /// <summary>Obtener tarea por ID - Devuelve el detalle de una tarea específica.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TareaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TareaResponseDto>> GetById(int id) =>
        Ok(await tareaService.GetByIdAsync(id));

    /// <summary>Crear tarea - Registra una nueva tarea asociada a un proyecto existente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TareaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TareaResponseDto>> Create(CreateTareaDto dto)
    {
        var creada = await tareaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    /// <summary>Actualizar tarea - Modifica todos los campos de una tarea existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TareaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TareaResponseDto>> Update(int id, UpdateTareaDto dto) =>
        Ok(await tareaService.UpdateAsync(id, dto));

    /// <summary>Cambiar estado de tarea - Actualiza únicamente el estado (Pendiente, EnProgreso o Completada).</summary>
    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(TareaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TareaResponseDto>> CambiarEstado(int id, CambiarEstadoTareaDto dto) =>
        Ok(await tareaService.CambiarEstadoAsync(id, dto));

    /// <summary>Eliminar tarea - Borra una tarea específica por su ID.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await tareaService.DeleteAsync(id);
        return NoContent();
    }
}
