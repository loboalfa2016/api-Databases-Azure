using GestionProyectos.Application.Exceptions;

namespace GestionProyectos.Application.xUnit.Exceptions;

public class ExceptionsTests
{
    [Fact]
    public void NotFoundException_GuardaMensajeCorrectamente()
    {
        var ex = new NotFoundException("Recurso no encontrado");
        Assert.Equal("Recurso no encontrado", ex.Message);
    }

    [Fact]
    public void BadRequestException_GuardaMensajeCorrectamente()
    {
        var ex = new BadRequestException("Fechas inválidas");
        Assert.Equal("Fechas inválidas", ex.Message);
    }

    [Fact]
    public void BusinessRuleException_GuardaMensajeCorrectamente()
    {
        var ex = new BusinessRuleException("Tiene tareas pendientes");
        Assert.Equal("Tiene tareas pendientes", ex.Message);
    }
}
