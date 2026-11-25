using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Application.Interfaces;

public interface ITareaRepository
{
    Task<IEnumerable<Tarea>> GetAllAsync(EstadoTarea? estado = null, PrioridadTarea? prioridad = null);
    Task<IEnumerable<Tarea>> GetByProyectoIdAsync(int proyectoId);
    Task<IEnumerable<Tarea>> GetVencidasAsync();
    Task<Tarea?> GetByIdAsync(int id);
    Task<Tarea> AddAsync(Tarea tarea);
    Task UpdateAsync(Tarea tarea);
    Task DeleteAsync(Tarea tarea);
}
