using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GestionProyectos.Application.DTOs;
using GestionProyectos.Domain.Enums;

namespace GestionProyectos.API.xUnit.Integration;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ApiIntegrationTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task FlujoCompleto_ProyectoYTarea_RespetaReglasDeNegocio()
    {
        var resProyecto = await _client.PostAsJsonAsync("/api/v1/proyectos",
            new CreateProyectoDto("Proyecto Integracion", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(30)));
        Assert.Equal(HttpStatusCode.Created, resProyecto.StatusCode);
        var proyecto = await resProyecto.Content.ReadFromJsonAsync<ProyectoResponseDto>(_jsonOptions);

        var resTarea = await _client.PostAsJsonAsync("/api/v1/tareas",
            new CreateTareaDto("Tarea Integracion", "Desc", PrioridadTarea.Alta, DateTime.UtcNow.AddDays(5), proyecto!.Id));
        Assert.Equal(HttpStatusCode.Created, resTarea.StatusCode);
        var tarea = await resTarea.Content.ReadFromJsonAsync<TareaResponseDto>(_jsonOptions);

        var resBloqueado = await _client.DeleteAsync($"/api/v1/proyectos/{proyecto.Id}");
        Assert.Equal(HttpStatusCode.Conflict, resBloqueado.StatusCode);

        var resPatch = await _client.PatchAsJsonAsync($"/api/v1/tareas/{tarea!.Id}/estado",
            new CambiarEstadoTareaDto(EstadoTarea.Completada));
        Assert.Equal(HttpStatusCode.OK, resPatch.StatusCode);

        var actualizado = await _client.GetFromJsonAsync<ProyectoResponseDto>($"/api/v1/proyectos/{proyecto.Id}", _jsonOptions);
        Assert.Equal(100, actualizado!.PorcentajeAvance);

        var resDeleteOk = await _client.DeleteAsync($"/api/v1/proyectos/{proyecto.Id}");
        Assert.Equal(HttpStatusCode.NoContent, resDeleteOk.StatusCode);
    }

    [Fact]
    public async Task GetProyectoInexistente_Retorna404NotFound()
    {
        var response = await _client.GetAsync("/api/v1/proyectos/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTareaInexistente_Retorna404NotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/tareas/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
