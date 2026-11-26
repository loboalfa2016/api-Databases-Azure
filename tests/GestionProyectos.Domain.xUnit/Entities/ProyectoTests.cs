using GestionProyectos.Domain.Entities;

namespace GestionProyectos.Domain.xUnit.Entities;

public class ProyectoTests
{
    [Fact]
    public void Proyecto_AlCrearse_InicializaListaDeTareasVacia()
    {
        var proyecto = new Proyecto();

        Assert.NotNull(proyecto.Tareas);
        Assert.Empty(proyecto.Tareas);
    }

    [Fact]
    public void Proyecto_AsignaPropiedadesCorrectamente()
    {
        var inicio = new DateTime(2026, 12, 1);
        var fin = new DateTime(2026, 12, 31);

        var proyecto = new Proyecto
        {
            Id = 1,
            Nombre = "API Gestion",
            Descripcion = "Backend .NET 10",
            FechaInicio = inicio,
            FechaFinalizacion = fin
        };

        Assert.Equal(1, proyecto.Id);
        Assert.Equal("API Gestion", proyecto.Nombre);
        Assert.Equal("Backend .NET 10", proyecto.Descripcion);
        Assert.Equal(inicio, proyecto.FechaInicio);
        Assert.Equal(fin, proyecto.FechaFinalizacion);
    }
}
