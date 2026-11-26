using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Domain.xUnit.Entities;

public class TareaTests
{
    [Fact]
    public void Tarea_AsignaPropiedadesYRelacionConProyecto()
    {
        var fechaLimite = new DateTime(2026, 12, 15);

        var tarea = new Tarea
        {
            Id = 10,
            Titulo = "Configurar Swagger",
            Descripcion = "Agregar comentarios XML",
            Estado = EstadoTarea.Pendiente,
            Prioridad = PrioridadTarea.Alta,
            FechaLimite = fechaLimite,
            ProyectoId = 1
        };

        Assert.Equal(10, tarea.Id);
        Assert.Equal("Configurar Swagger", tarea.Titulo);
        Assert.Equal(EstadoTarea.Pendiente, tarea.Estado);
        Assert.Equal(PrioridadTarea.Alta, tarea.Prioridad);
        Assert.Equal(fechaLimite, tarea.FechaLimite);
        Assert.Equal(1, tarea.ProyectoId);
    }
}
