using GestionProyectos.Application.DTOs;

namespace GestionProyectos.Application.Interfaces;

public interface IProyectoService
{
    Task<IEnumerable<ProyectoResponseDto>> GetAllAsync();
    Task<ProyectoResponseDto> GetByIdAsync(int id);
    Task<ProyectoResponseDto> CreateAsync(CreateProyectoDto dto);
    Task<ProyectoResponseDto> UpdateAsync(int id, UpdateProyectoDto dto);
    Task DeleteAsync(int id);
}