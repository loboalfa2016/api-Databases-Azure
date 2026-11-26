using GestionProyectos.Domain.Enums;

namespace GestionProyectos.Domain.xUnit.Enums;

public class EstadoTareaTests
{
    [Fact]
    public void EstadoTarea_ContieneValoresEsperados()
    {
        var valores = Enum.GetNames<EstadoTarea>();

        Assert.Contains("Pendiente", valores);
        Assert.Contains("EnProgreso", valores);
        Assert.Contains("Completada", valores);
        Assert.Equal(3, valores.Length);
    }
}
