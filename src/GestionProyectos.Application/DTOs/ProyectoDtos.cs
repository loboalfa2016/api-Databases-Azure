namespace GestionProyectos.Application.DTOs;

public record CreateProyectoDto(
    string Nombre,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaFinalizacion
);

public record UpdateProyectoDto(
    string Nombre,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaFinalizacion
);

public record ProyectoResponseDto(
    int Id,
    string Nombre,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaFinalizacion,
    double PorcentajeAvance,
    int TotalTareas
);