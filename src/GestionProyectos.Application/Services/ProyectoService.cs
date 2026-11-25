using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Exceptions;
using GestionProyectos.Application.Interfaces;
using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Application.Services;

public class ProyectoService(IProyectoRepository repository) : IProyectoService
{
    public async Task<IEnumerable<ProyectoResponseDto>> GetAllAsync()
    {
        var proyectos = await repository.GetAllAsync();
        return proyectos.Select(MapToDto);
    }

    public async Task<ProyectoResponseDto> GetByIdAsync(int id)
    {
        var proyecto = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el proyecto con ID {id}.");
        return MapToDto(proyecto);
    }

    public async Task<ProyectoResponseDto> CreateAsync(CreateProyectoDto dto)
    {
        ValidarProyecto(dto.Nombre, dto.Descripcion, dto.FechaInicio, dto.FechaFinalizacion);

        var proyecto = new Proyecto
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            FechaInicio = DateTime.SpecifyKind(dto.FechaInicio, DateTimeKind.Utc),
            FechaFinalizacion = DateTime.SpecifyKind(dto.FechaFinalizacion, DateTimeKind.Utc)
        };

        var creado = await repository.AddAsync(proyecto);
        return MapToDto(creado);
    }

    public async Task<ProyectoResponseDto> UpdateAsync(int id, UpdateProyectoDto dto)
    {
        ValidarProyecto(dto.Nombre, dto.Descripcion, dto.FechaInicio, dto.FechaFinalizacion);

        var proyecto = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el proyecto con ID {id}.");

        proyecto.Nombre = dto.Nombre.Trim();
        proyecto.Descripcion = dto.Descripcion?.Trim();
        proyecto.FechaInicio = DateTime.SpecifyKind(dto.FechaInicio, DateTimeKind.Utc);
        proyecto.FechaFinalizacion = DateTime.SpecifyKind(dto.FechaFinalizacion, DateTimeKind.Utc);

        await repository.UpdateAsync(proyecto);
        return MapToDto(proyecto);
    }

    public async Task DeleteAsync(int id)
    {
        var proyecto = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el proyecto con ID {id}.");

        if (proyecto.Tareas.Any(t => t.Estado == EstadoTarea.Pendiente))
            throw new BusinessRuleException("No se puede eliminar un proyecto que tiene tareas pendientes.");

        await repository.DeleteAsync(proyecto);
    }

    private static void ValidarProyecto(string nombre, string? descripcion, DateTime inicio, DateTime fin)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new BadRequestException("El nombre del proyecto es obligatorio.");

        if (nombre.Length > 150)
            throw new BadRequestException("El nombre del proyecto no puede superar los 150 caracteres.");

        if (descripcion is { Length: > 1000 })
            throw new BadRequestException("La descripción no puede superar los 1000 caracteres.");

        if (fin < inicio)
            throw new BadRequestException("La fecha de finalización no puede ser anterior a la fecha de inicio.");
    }

    private static ProyectoResponseDto MapToDto(Proyecto p) => new(
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.FechaInicio,
        p.FechaFinalizacion,
        p.CalcularPorcentajeAvance(),
        p.Tareas.Count
    );
}