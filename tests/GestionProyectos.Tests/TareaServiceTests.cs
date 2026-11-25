using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Exceptions;
using GestionProyectos.Application.Interfaces;
using GestionProyectos.Application.Services;
using GestionProyectos.Domain.Entities;
using GestionProyectos.Domain.Enums;
using Moq;

namespace GestionProyectos.Tests;

public class TareaServiceTests
{
    private readonly Mock<ITareaRepository> _tareaRepoMock = new();
    private readonly Mock<IProyectoRepository> _proyectoRepoMock = new();
    private readonly TareaService _service;

    public TareaServiceTests()
    {
        _service = new TareaService(_tareaRepoMock.Object, _proyectoRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ProyectoInexistente_LanzaNotFoundException()
    {
        _proyectoRepoMock.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        var dto = new CreateTareaDto("Nueva tarea", "Desc", PrioridadTarea.Alta, DateTime.UtcNow.AddDays(5), 99);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CambiarEstadoAsync_EstadoValido_ActualizaEstado()
    {
        var tarea = new Tarea { Id = 1, Titulo = "Test", Estado = EstadoTarea.Pendiente, ProyectoId = 1 };
        _tareaRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tarea);

        var resultado = await _service.CambiarEstadoAsync(1, new CambiarEstadoTareaDto(EstadoTarea.Completada));

        Assert.Equal(EstadoTarea.Completada, resultado.Estado);
        _tareaRepoMock.Verify(r => r.UpdateAsync(tarea), Times.Once);
    }
}
