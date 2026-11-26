using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;
using GestionProyectos.Infrastructure.Persistence;
using GestionProyectos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Infrastructure.xUnit.Repositories;

public class ProyectoRepositoryTests
{
    private static AppDbContext CreateContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"InfraProyectoDb_{Guid.NewGuid()}")
            .Options);

    [Fact]
    public async Task AddYGetByIdAsync_PersisteProyectoConSusTareas()
    {
        using var context = CreateContext();
        var repo = new ProyectoRepository(context);

        var guardado = await repo.AddAsync(new Proyecto
        {
            Nombre = "Proyecto Infra",
            FechaInicio = DateTime.UtcNow,
            FechaFinalizacion = DateTime.UtcNow.AddDays(10),
            Tareas = [new Tarea { Titulo = "T1", Prioridad = PrioridadTarea.Alta, Estado = EstadoTarea.Pendiente, FechaLimite = DateTime.UtcNow.AddDays(3) }]
        });

        var obtenido = await repo.GetByIdAsync(guardado.Id);
        Assert.NotNull(obtenido);
        Assert.Single(obtenido.Tareas);
    }

    [Fact]
    public async Task DeleteAsync_EliminaProyectoDeLaBaseDeDatos()
    {
        using var context = CreateContext();
        var repo = new ProyectoRepository(context);

        var guardado = await repo.AddAsync(new Proyecto { Nombre = "Borrar", FechaInicio = DateTime.UtcNow, FechaFinalizacion = DateTime.UtcNow.AddDays(5) });
        await repo.DeleteAsync(guardado);

        Assert.Null(await repo.GetByIdAsync(guardado.Id));
    }
}
