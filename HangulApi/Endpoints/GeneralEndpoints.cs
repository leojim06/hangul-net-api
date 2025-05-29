using HangulApi.Features.General;
using MediatR;

namespace HangulApi.Endpoints;

public static class GeneralEndpoints
{
    public static RouteGroupBuilder MapGeneralEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("api/v1/general");

        ConfigureSummaryEndpoint(group);
        ConfigureGetJamoTypesEndpoint(group);

        return group;
    }

    private static void ConfigureGetJamoTypesEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/types", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var types = await sender.Send(new GetJamoTypes.Query(), cancellationToken);
            return Results.Ok(types);
        })
        .WithName("Lista tipos de Jamo")
        .WithDescription("Obtiene los tipos de carácter disponibles en el sistema")
        .WithTags("General");
    }

    private static void ConfigureSummaryEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/summary", async (
        ISender sender,
        CancellationToken cancellationToken) =>
        {
            var summary = await sender.Send(new HangulSummary.Query(), cancellationToken);
            return Results.Ok(summary);
        })
        .WithName("Resumen idioma Coreano")
        .WithDescription("Obtiene introduccion del idioma Coreano, además del número de caracteres agrupados por vocales, consonantes y grupos consonánticos registrados en la aplicación.")
        .WithTags("General");
    }
}
