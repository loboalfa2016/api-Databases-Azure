using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Exceptions;
using GestionProyectos.Application.Interfaces;
using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Application.Services;

public class TareaService(ITareaRepository tareaRepo, IProyectoRepository proyectoRepo) : ITareaService
{
    public async Task<IEnumerable<TareaResponseDto>> GetAllAsync(EstadoTarea? estado = null, PrioridadTarea? prioridad = null)
    {
        var tareas = await tareaRepo.GetAllAsync(estado, prioridad);
        return tareas.Select(MapToDto);
    }

    public async Task<IEnumerable<TareaResponseDto>> GetByProyectoIdAsync(int proyectoId)
    {
        if (!await proyectoRepo.ExistsAsync(proyectoId))
            throw new NotFoundException($"No se encontró el proyecto con ID {proyectoId}.");

        var tareas = await tareaRepo.GetByProyectoIdAsync(proyectoId);
        return tareas.Select(MapToDto);
    }

    public async Task<IEnumerable<TareaResponseDto>> GetVencidasAsync()
    {
        var tareas = await tareaRepo.GetVencidasAsync();
        return tareas.Select(MapToDto);
    }

    public async Task<TareaResponseDto> GetByIdAsync(int id)
    {
        var tarea = await tareaRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró la tarea con ID {id}.");
        return MapToDto(tarea);
    }

    public async Task<TareaResponseDto> CreateAsync(CreateTareaDto dto)
    {
        ValidarTarea(dto.Titulo, dto.Descripcion, EstadoTarea.Pendiente, dto.Prioridad);

        if (!await proyectoRepo.ExistsAsync(dto.ProyectoId))
            throw new NotFoundException($"El proyecto con ID {dto.ProyectoId} no existe.");

        var tarea = new Tarea
        {
            Titulo = dto.Titulo.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            Estado = EstadoTarea.Pendiente,
            Prioridad = dto.Prioridad,
            FechaLimite = DateTime.SpecifyKind(dto.FechaLimite, DateTimeKind.Utc),
            ProyectoId = dto.ProyectoId
        };

        var creada = await tareaRepo.AddAsync(tarea);
        return MapToDto(creada);
    }

    public async Task<TareaResponseDto> UpdateAsync(int id, UpdateTareaDto dto)
    {
        ValidarTarea(dto.Titulo, dto.Descripcion, dto.Estado, dto.Prioridad);

        var tarea = await tareaRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró la tarea con ID {id}.");

        tarea.Titulo = dto.Titulo.Trim();
        tarea.Descripcion = dto.Descripcion?.Trim();
        tarea.Estado = dto.Estado;
        tarea.Prioridad = dto.Prioridad;
        tarea.FechaLimite = DateTime.SpecifyKind(dto.FechaLimite, DateTimeKind.Utc);

        await tareaRepo.UpdateAsync(tarea);
        return MapToDto(tarea);
    }

    public async Task<TareaResponseDto> CambiarEstadoAsync(int id, CambiarEstadoTareaDto dto)
    {
        if (!Enum.IsDefined(dto.Estado))
            throw new BadRequestException("El estado proporcionado no es válido.");

        var tarea = await tareaRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró la tarea con ID {id}.");

        tarea.Estado = dto.Estado;
        await tareaRepo.UpdateAsync(tarea);
        return MapToDto(tarea);
    }

    public async Task DeleteAsync(int id)
    {
        var tarea = await tareaRepo.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró la tarea con ID {id}.");

        await tareaRepo.DeleteAsync(tarea);
    }

    private static void ValidarTarea(string titulo, string? descripcion, EstadoTarea estado, PrioridadTarea prioridad)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new BadRequestException("El título de la tarea es obligatorio.");

        if (titulo.Length > 150)
            throw new BadRequestException("El título no puede superar los 150 caracteres.");

        if (descripcion is { Length: > 1000 })
            throw new BadRequestException("La descripción no puede superar los 1000 caracteres.");

        if (!Enum.IsDefined(estado))
            throw new BadRequestException("El estado de la tarea no es válido.");

        if (!Enum.IsDefined(prioridad))
            throw new BadRequestException("La prioridad de la tarea no es válida.");
    }

    private static TareaResponseDto MapToDto(Tarea t) => new(
        t.Id,
        t.Titulo,
        t.Descripcion,
        t.Estado,
        t.Prioridad,
        t.FechaLimite,
        t.ProyectoId
    );
}