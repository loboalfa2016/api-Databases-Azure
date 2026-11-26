using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Exceptions;
using GestionProyectos.Application.Interfaces;
using GestionProyectos.Application.Services;
using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;
using Moq;

namespace GestionProyectos.Tests;

public class ProyectoServiceTests
{
    private readonly Mock<IProyectoRepository> _repoMock = new();
    private readonly ProyectoService _service;

    public ProyectoServiceTests()
    {
        _service = new ProyectoService(_repoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_DatosValidos_CreaYRetornaProyecto()
    {
        var dto = new CreateProyectoDto(
            "Proyecto Valido",
            "Descripcion",
            new DateTime(2026, 12, 1),
            new DateTime(2026, 12, 20)
        );

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Proyecto>()))
            .ReturnsAsync((Proyecto p) => { p.Id = 1; return p; });

        var resultado = await _service.CreateAsync(dto);

        Assert.Equal(1, resultado.Id);
        Assert.Equal("Proyecto Valido", resultado.Nombre);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Proyecto>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_FechaFinAnteriorAInicio_LanzaBadRequest()
    {
        var dto = new CreateProyectoDto(
            "Proyecto Inválido",
            "Desc",
            new DateTime(2026, 12, 10),
            new DateTime(2026, 12, 1)
        );

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task GetByIdAsync_NoExiste_LanzaNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Proyecto?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetByIdAsync_CalculaPorcentajeAvanceCorrectamente()
    {
        var proyecto = new Proyecto
        {
            Id = 1,
            Nombre = "Proyecto Avance",
            Tareas =
            [
                new Tarea { Estado = EstadoTarea.Completada },
                new Tarea { Estado = EstadoTarea.Pendiente }
            ]
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(proyecto);

        var resultado = await _service.GetByIdAsync(1);

        Assert.Equal(50, resultado.PorcentajeAvance);
        Assert.Equal(2, resultado.TotalTareas);
    }

    [Fact]
    public async Task DeleteAsync_ProyectoConTareaPendiente_LanzaBusinessRuleException()
    {
        var proyecto = new Proyecto
        {
            Id = 1,
            Nombre = "Proyecto Activo",
            Tareas = [new Tarea { Id = 10, Titulo = "Pendiente", Estado = EstadoTarea.Pendiente }]
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(proyecto);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Proyecto>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_SinTareasPendientes_EliminaCorrectamente()
    {
        var proyecto = new Proyecto
        {
            Id = 1,
            Nombre = "Proyecto Listo",
            Tareas = [new Tarea { Id = 10, Titulo = "Hecha", Estado = EstadoTarea.Completada }]
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(proyecto);

        await _service.DeleteAsync(1);

        _repoMock.Verify(r => r.DeleteAsync(proyecto), Times.Once);
    }
}
