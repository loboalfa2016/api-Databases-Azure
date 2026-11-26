using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;
using GestionProyectos.Infrastructure.Persistence;
using GestionProyectos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Infrastructure.xUnit.Repositories;

public class TareaRepositoryTests
{
    private static AppDbContext CreateContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"InfraTareaDb_{Guid.NewGuid()}")
            .Options);

    [Fact]
    public async Task GetAllAsync_FiltraPorEstadoYPrioridad()
    {
        using var context = CreateContext();
        var proyecto = new Proyecto { Nombre = "P1", FechaInicio = DateTime.UtcNow, FechaFinalizacion = DateTime.UtcNow.AddDays(10) };
        context.Proyectos.Add(proyecto);
        await context.SaveChangesAsync();

        var repo = new TareaRepository(context);
        await repo.AddAsync(new Tarea { Titulo = "T1", Estado = EstadoTarea.Pendiente, Prioridad = PrioridadTarea.Alta, FechaLimite = DateTime.UtcNow.AddDays(2), ProyectoId = proyecto.Id });
        await repo.AddAsync(new Tarea { Titulo = "T2", Estado = EstadoTarea.Completada, Prioridad = PrioridadTarea.Alta, FechaLimite = DateTime.UtcNow.AddDays(2), ProyectoId = proyecto.Id });

        var resultado = (await repo.GetAllAsync(EstadoTarea.Pendiente, PrioridadTarea.Alta)).ToList();
        Assert.Single(resultado);
        Assert.Equal("T1", resultado[0].Titulo);
    }

    [Fact]
    public async Task GetByIdAsync_RetornaTareaSiExiste()
    {
        using var context = CreateContext();
        var proyecto = new Proyecto { Nombre = "P", FechaInicio = DateTime.UtcNow, FechaFinalizacion = DateTime.UtcNow.AddDays(5) };
        context.Proyectos.Add(proyecto);
        await context.SaveChangesAsync();

        var repo = new TareaRepository(context);
        var creada = await repo.AddAsync(new Tarea { Titulo = "Buscar", Estado = EstadoTarea.Pendiente, Prioridad = PrioridadTarea.Media, FechaLimite = DateTime.UtcNow.AddDays(2), ProyectoId = proyecto.Id });

        var encontrada = await repo.GetByIdAsync(creada.Id);
        Assert.NotNull(encontrada);
        Assert.Equal("Buscar", encontrada.Titulo);
    }

    [Fact]
    public async Task GetVencidasAsync_RetornaSoloVencidasNoCompletadas()
    {
        using var context = CreateContext();
        var proyecto = new Proyecto { Nombre = "P2", FechaInicio = DateTime.UtcNow.AddDays(-5), FechaFinalizacion = DateTime.UtcNow.AddDays(10) };
        context.Proyectos.Add(proyecto);
        await context.SaveChangesAsync();

        var repo = new TareaRepository(context);
        await repo.AddAsync(new Tarea { Titulo = "Vencida", Estado = EstadoTarea.Pendiente, Prioridad = PrioridadTarea.Alta, FechaLimite = DateTime.UtcNow.AddDays(-1), ProyectoId = proyecto.Id });
        await repo.AddAsync(new Tarea { Titulo = "Futura", Estado = EstadoTarea.Pendiente, Prioridad = PrioridadTarea.Media, FechaLimite = DateTime.UtcNow.AddDays(5), ProyectoId = proyecto.Id });

        var vencidas = (await repo.GetVencidasAsync()).ToList();
        Assert.Single(vencidas);
        Assert.Equal("Vencida", vencidas[0].Titulo);
    }

    [Fact]
    public async Task UpdateAsync_ActualizaEstadoYTitulo()
    {
        using var context = CreateContext();
        var proyecto = new Proyecto { Nombre = "P", FechaInicio = DateTime.UtcNow, FechaFinalizacion = DateTime.UtcNow.AddDays(5) };
        context.Proyectos.Add(proyecto);
        await context.SaveChangesAsync();

        var repo = new TareaRepository(context);
        var creada = await repo.AddAsync(new Tarea { Titulo = "Inicial", Estado = EstadoTarea.Pendiente, Prioridad = PrioridadTarea.Baja, FechaLimite = DateTime.UtcNow.AddDays(2), ProyectoId = proyecto.Id });

        creada.Titulo = "Modificada";
        creada.Estado = EstadoTarea.Completada;
        await repo.UpdateAsync(creada);

        var actualizada = await repo.GetByIdAsync(creada.Id);
        Assert.Equal("Modificada", actualizada!.Titulo);
        Assert.Equal(EstadoTarea.Completada, actualizada.Estado);
    }

    [Fact]
    public async Task DeleteAsync_EliminaTareaDeLaBaseDeDatos()
    {
        using var context = CreateContext();
        var proyecto = new Proyecto { Nombre = "P", FechaInicio = DateTime.UtcNow, FechaFinalizacion = DateTime.UtcNow.AddDays(5) };
        context.Proyectos.Add(proyecto);
        await context.SaveChangesAsync();

        var repo = new TareaRepository(context);
        var creada = await repo.AddAsync(new Tarea { Titulo = "Borrar", Estado = EstadoTarea.Pendiente, Prioridad = PrioridadTarea.Media, FechaLimite = DateTime.UtcNow.AddDays(2), ProyectoId = proyecto.Id });

        await repo.DeleteAsync(creada);

        Assert.Null(await repo.GetByIdAsync(creada.Id));
    }
}
