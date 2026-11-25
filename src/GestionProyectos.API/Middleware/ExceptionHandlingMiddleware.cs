using System.Net;
using System.Text.Json;
using GestionProyectos.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            BadRequestException ex => (HttpStatusCode.BadRequest, "Solicitud incorrecta", ex.Message),
            NotFoundException ex => (HttpStatusCode.NotFound, "Recurso no encontrado", ex.Message),
            BusinessRuleException ex => (HttpStatusCode.Conflict, "Conflicto de regla de negocio", ex.Message),
            DbUpdateException => (HttpStatusCode.Conflict, "Error de base de datos", "No se pudo completar la operación por una restricción de datos."),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor", "Ocurrió un error inesperado.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}
