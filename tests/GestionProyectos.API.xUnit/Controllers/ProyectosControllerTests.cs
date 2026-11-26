using GestionProyectos.API.Controllers;
using GestionProyectos.Application.DTOs;
using GestionProyectos.Application.Interfaces;
using GestionProyectos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GestionProyectos.API.xUnit.Controllers;

public class ProyectosControllerTests
{
    private readonly Mock<IProyectoService> _proyectoServiceMock = new();
    private readonly Mock<ITareaService> _tareaServiceMock = new();
    private readonly ProyectosController _controller;

    public ProyectosControllerTests() =>
        _controller = new ProyectosController(_proyectoServiceMock.Object, _tareaServiceMock.Object);

    [Fact]
    public async Task GetAll_RetornaOkConListaDeProyectos()
    {
        var lista = new List<ProyectoResponseDto>
        {
            new(1, "Proyecto 1", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(5), 0, 0)
        };
        _proyectoServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(lista);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<ProyectoResponseDto>>(ok.Value));
    }

    [Fact]
    public async Task GetById_RetornaOkConProyecto()
    {
        var dto = new ProyectoResponseDto(1, "P1", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(5), 2, 50);
        _proyectoServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(dto);

        var result = await _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetTareas_RetornaOkConTareasDelProyecto()
    {
        var tareas = new List<TareaResponseDto>
        {
            new(1, "T1", "Desc", EstadoTarea.Pendiente, PrioridadTarea.Alta, DateTime.UtcNow.AddDays(2), 1)
        };
        _tareaServiceMock.Setup(s => s.GetByProyectoIdAsync(1)).ReturnsAsync(tareas);

        var result = await _controller.GetTareasByProyecto(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<TareaResponseDto>>(ok.Value);
    }

    [Fact]
    public async Task Create_RetornaCreatedAtAction()
    {
        var createDto = new CreateProyectoDto("Nuevo", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
        var responseDto = new ProyectoResponseDto(1, "Nuevo", "Desc", createDto.FechaInicio, createDto.FechaFinalizacion, 0, 0);
        _proyectoServiceMock.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(responseDto);

        var result = await _controller.Create(createDto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(responseDto, created.Value);
    }

    [Fact]
    public async Task Update_RetornaOkConProyectoActualizado()
    {
        var updateDto = new UpdateProyectoDto("Editado", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(15));
        var responseDto = new ProyectoResponseDto(1, "Editado", "Desc", updateDto.FechaInicio, updateDto.FechaFinalizacion, 0, 0);
        _proyectoServiceMock.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(responseDto);

        var result = await _controller.Update(1, updateDto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(responseDto, ok.Value);
    }

    [Fact]
    public async Task Delete_RetornaNoContent()
    {
        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        _proyectoServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}