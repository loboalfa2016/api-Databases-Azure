using GestionProyectos.Application.DTOs;
using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Application.xUnit.DTOs;

public class DtoTests
{
    [Fact]
    public void CreateProyectoDto_ConservaValoresAsignados()
    {
        var inicio = new DateTime(2026, 12, 1);
        var fin = new DateTime(2026, 12, 20);

        var dto = new CreateProyectoDto("Proyecto A", "Desc A", inicio, fin);

        Assert.Equal("Proyecto A", dto.Nombre);
        Assert.Equal("Desc A", dto.Descripcion);
        Assert.Equal(inicio, dto.FechaInicio);
        Assert.Equal(fin, dto.FechaFinalizacion);
    }

    [Fact]
    public void CreateTareaDto_ConservaValoresAsignados()
    {
        var limite = new DateTime(2026, 12, 10);

        var dto = new CreateTareaDto("Tarea A", "Desc", PrioridadTarea.Alta, limite, 1);

        Assert.Equal("Tarea A", dto.Titulo);
        Assert.Equal(PrioridadTarea.Alta, dto.Prioridad);
        Assert.Equal(limite, dto.FechaLimite);
        Assert.Equal(1, dto.ProyectoId);
    }
}
