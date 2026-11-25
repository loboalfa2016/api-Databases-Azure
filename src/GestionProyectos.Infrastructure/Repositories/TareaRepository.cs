using GestionProyectos.Application.Interfaces;
using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;
using GestionProyectos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Infrastructure.Repositories;

public class TareaRepository(AppDbContext context) : ITareaRepository
{
    public async Task<IEnumerable<Tarea>> GetAllAsync(EstadoTarea? estado = null, PrioridadTarea? prioridad = null)
    {
        var query = context.Tareas.AsNoTracking().AsQueryable();

        if (estado.HasValue)
            query = query.Where(t => t.Estado == estado.Value);

        if (prioridad.HasValue)
            query = query.Where(t => t.Prioridad == prioridad.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Tarea>> GetByProyectoIdAsync(int proyectoId) =>
        await context.Tareas.AsNoTracking().Where(t => t.ProyectoId == proyectoId).ToListAsync();

    public async Task<IEnumerable<Tarea>> GetVencidasAsync() =>
        await context.Tareas.AsNoTracking()
            .Where(t => t.FechaLimite < DateTime.UtcNow && t.Estado != EstadoTarea.Completada)
            .ToListAsync();

    public async Task<Tarea?> GetByIdAsync(int id) =>
        await context.Tareas.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Tarea> AddAsync(Tarea tarea)
    {
        context.Tareas.Add(tarea);
        await context.SaveChangesAsync();
        return tarea;
    }

    public async Task UpdateAsync(Tarea tarea)
    {
        context.Tareas.Update(tarea);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tarea tarea)
    {
        context.Tareas.Remove(tarea);
        await context.SaveChangesAsync();
    }
}