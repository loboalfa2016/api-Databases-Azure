using System.Net;
using System.Net.Http.Json;
using GestionProyectos.API.xUnit.Integration;
using GestionProyectos.Application.DTOs;

namespace GestionProyectos.API.xUnit.Middleware;

public class MiddlewareYSwaggerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MiddlewareYSwaggerTests(CustomWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task SwaggerJson_Responde200OK()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostProyectoFechasInvalidas_MiddlewareRetorna400ProblemDetails()
    {
        var dtoInvalido = new CreateProyectoDto("Invalido", "Desc", new DateTime(2026, 12, 20), new DateTime(2026, 12, 1));
        var response = await _client.PostAsJsonAsync("/api/v1/proyectos", dtoInvalido);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
