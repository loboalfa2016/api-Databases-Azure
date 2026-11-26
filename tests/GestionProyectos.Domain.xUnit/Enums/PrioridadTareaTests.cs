using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Domain.xUnit.Enums;

public class PrioridadTareaTests
{
    [Fact]
    public void PrioridadTarea_ContieneValoresEsperados()
    {
        var valores = Enum.GetNames<PrioridadTarea>();

        Assert.Contains("Baja", valores);
        Assert.Contains("Media", valores);
        Assert.Contains("Alta", valores);
        Assert.Equal(3, valores.Length);
    }
}
