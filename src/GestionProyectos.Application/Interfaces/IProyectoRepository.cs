using GestionProyectos.Domain.Entities;

namespace GestionProyectos.Application.Interfaces;

public interface IProyectoRepository
{
    Task<IEnumerable<Proyecto>> GetAllAsync();
    Task<Proyecto?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<Proyecto> AddAsync(Proyecto proyecto);
    Task UpdateAsync(Proyecto proyecto);
    Task DeleteAsync(Proyecto proyecto);
}
