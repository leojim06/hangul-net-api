using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HangulApi.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            try
            {
                await next();

                if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
                {
                    await WriteProblemDetailsAsync(context, new ProblemDetails
                    {
                        Status = 404,
                        Title = "Recurso no encontrado",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                        Detail = $"La ruta {context.Request.Path} no existe.",
                        Instance = context.Request.Path
                    });
                }
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                await WriteProblemDetailsAsync(context, new ProblemDetails
                {
                    Status = 400,
                    Title = "Errores de validación",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Detail = "Uno o más errores ocurrieron al validar su solicitud.",
                    Instance = context.Request.Path,
                    Errors = errors
                });
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
            {
                await WriteProblemDetailsAsync(context, new ProblemDetails
                {
                    Status = 400,
                    Title = "Violación de restricción de unicidad",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Detail = "Ya existe un recurso con el mismo valor único.",
                    Instance = context.Request.Path
                });
            }
            catch (Exception ex)
            {
                await WriteProblemDetailsAsync(context, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error interno del servidor",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                });
            }
        });
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, ProblemDetails problem)
    {
        context.Response.StatusCode = problem.Status ?? 500;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }
}
