using GestionProyectos.Domain.Entities;
using GestionProyectos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Infrastructure.xUnit.Persistence;

public class AppDbContextTests
{
    [Fact]
    public async Task AppDbContext_PuedeGuardarYConsultarProyectosYTareas()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"DbContextTest_{Guid.NewGuid()}")
            .Options;

        using var context = new AppDbContext(options);

        var proyecto = new Proyecto
        {
            Nombre = "Proyecto DbContext",
            FechaInicio = DateTime.UtcNow,
            FechaFinalizacion = DateTime.UtcNow.AddDays(10)
        };

        context.Proyectos.Add(proyecto);
        await context.SaveChangesAsync();

        Assert.Equal(1, await context.Proyectos.CountAsync());
    }
}
