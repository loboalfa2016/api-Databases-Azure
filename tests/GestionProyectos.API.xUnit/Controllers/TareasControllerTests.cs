using GestionProyectos.API.Controllers;
using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Interfaces;
using GestionProyectos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GestionProyectos.API.xUnit.Controllers;

public class TareasControllerTests
{
    private readonly Mock<ITareaService> _serviceMock = new();
    private readonly TareasController _controller;

    public TareasControllerTests() => _controller = new TareasController(_serviceMock.Object);

    [Fact]
    public async Task GetAll_RetornaOkConTareas()
    {
        var tareas = new List<TareaResponseDto>
        {
            new(1, "Tarea 1", "Desc", EstadoTarea.Pendiente, PrioridadTarea.Alta, DateTime.UtcNow.AddDays(2), 1)
        };
        _serviceMock.Setup(s => s.GetAllAsync(null, null)).ReturnsAsync(tareas);

        var result = await _controller.GetAll(null, null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<TareaResponseDto>>(ok.Value);
        Assert.Single(data);
    }

    [Fact]
    public async Task GetVencidas_RetornaOkConTareasVencidas()
    {
        var vencidas = new List<TareaResponseDto>
        {
            new(2, "Vencida", "Desc", EstadoTarea.Pendiente, PrioridadTarea.Alta, DateTime.UtcNow.AddDays(-1), 1)
        };
        _serviceMock.Setup(s => s.GetVencidasAsync()).ReturnsAsync(vencidas);

        var result = await _controller.GetVencidas();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<TareaResponseDto>>(ok.Value));
    }

    [Fact]
    public async Task GetById_RetornaOkConTarea()
    {
        var dto = new TareaResponseDto(1, "T1", "Desc", EstadoTarea.Pendiente, PrioridadTarea.Media, DateTime.UtcNow.AddDays(2), 1);
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

        var result = await _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task Create_RetornaCreatedAtAction()
    {
        var createDto = new CreateTareaDto("Nueva", "Desc", PrioridadTarea.Alta, DateTime.UtcNow.AddDays(3), 1);
        var responseDto = new TareaResponseDto(1, "Nueva", "Desc", EstadoTarea.Pendiente, PrioridadTarea.Alta, createDto.FechaLimite, 1);
        _serviceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(responseDto);

        var result = await _controller.Create(createDto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(responseDto, created.Value);
    }

    [Fact]
    public async Task Update_RetornaOkConTareaActualizada()
    {
        var updateDto = new UpdateTareaDto("Editada", "Desc", EstadoTarea.Pendiente, PrioridadTarea.Baja, DateTime.UtcNow.AddDays(5));
        var responseDto = new TareaResponseDto(1, "Editada", "Desc", EstadoTarea.Pendiente, PrioridadTarea.Baja, updateDto.FechaLimite, 1);
        _serviceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(responseDto);

        var result = await _controller.Update(1, updateDto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(responseDto, ok.Value);
    }

    [Fact]
    public async Task CambiarEstado_RetornaOkConNuevoEstado()
    {
        var estadoDto = new CambiarEstadoTareaDto(EstadoTarea.Completada);
        var responseDto = new TareaResponseDto(1, "T1", "Desc", EstadoTarea.Completada, PrioridadTarea.Alta, DateTime.UtcNow.AddDays(2), 1);
        _serviceMock.Setup(s => s.CambiarEstadoAsync(1, estadoDto)).ReturnsAsync(responseDto);

        var result = await _controller.CambiarEstado(1, estadoDto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(responseDto, ok.Value);
    }

    [Fact]
    public async Task Delete_RetornaNoContent()
    {
        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
