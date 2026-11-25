using GestionProyectos.Application.DTOs;
using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Application.Interfaces;

public interface ITareaService
{
    Task<IEnumerable<TareaResponseDto>> GetAllAsync(EstadoTarea? estado = null, PrioridadTarea? prioridad = null);
    Task<IEnumerable<TareaResponseDto>> GetByProyectoIdAsync(int proyectoId);
    Task<IEnumerable<TareaResponseDto>> GetVencidasAsync();
    Task<TareaResponseDto> GetByIdAsync(int id);
    Task<TareaResponseDto> CreateAsync(CreateTareaDto dto);
    Task<TareaResponseDto> UpdateAsync(int id, UpdateTareaDto dto);
    Task<TareaResponseDto> CambiarEstadoAsync(int id, CambiarEstadoTareaDto dto);
    Task DeleteAsync(int id);
}