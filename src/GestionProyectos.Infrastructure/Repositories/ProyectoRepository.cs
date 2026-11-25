using GestionProyectos.Application.Interfaces;
using GestionProyectos.Domain.Entities;
using GestionProyectos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Infrastructure.Repositories;

public class ProyectoRepository(AppDbContext context) : IProyectoRepository
{
    public async Task<IEnumerable<Proyecto>> GetAllAsync() =>
        await context.Proyectos.Include(p => p.Tareas).AsNoTracking().ToListAsync();

    public async Task<Proyecto?> GetByIdAsync(int id) =>
        await context.Proyectos.Include(p => p.Tareas).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<bool> ExistsAsync(int id) =>
        await context.Proyectos.AnyAsync(p => p.Id == id);

    public async Task<Proyecto> AddAsync(Proyecto proyecto)
    {
        context.Proyectos.Add(proyecto);
        await context.SaveChangesAsync();
        return proyecto;
    }

    public async Task UpdateAsync(Proyecto proyecto)
    {
        context.Proyectos.Update(proyecto);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Proyecto proyecto)
    {
        context.Proyectos.Remove(proyecto);
        await context.SaveChangesAsync();
    }
}