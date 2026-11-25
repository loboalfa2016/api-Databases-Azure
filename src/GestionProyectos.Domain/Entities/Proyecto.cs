namespace GestionProyectos.Domain.Entities;

public class Proyecto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFinalizacion { get; set; }

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public double CalcularPorcentajeAvance()
    {
        if (Tareas.Count == 0) return 0;
        var completadas = Tareas.Count(t => t.Estado == Enums.EstadoTarea.Completada);
        return Math.Round((double)completadas / Tareas.Count * 100, 2);
    }
}