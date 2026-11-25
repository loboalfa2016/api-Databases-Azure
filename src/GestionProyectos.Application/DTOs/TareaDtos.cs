using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Application.DTOs;

public record CreateTareaDto(
    string Titulo,
    string? Descripcion,
    PrioridadTarea Prioridad,
    DateTime FechaLimite,
    int ProyectoId
);

public record UpdateTareaDto(
    string Titulo,
    string? Descripcion,
    EstadoTarea Estado,
    PrioridadTarea Prioridad,
    DateTime FechaLimite
);

public record CambiarEstadoTareaDto(EstadoTarea Estado);

public record TareaResponseDto(
    int Id,
    string Titulo,
    string? Descripcion,
    EstadoTarea Estado,
    PrioridadTarea Prioridad,
    DateTime FechaLimite,
    int ProyectoId
);