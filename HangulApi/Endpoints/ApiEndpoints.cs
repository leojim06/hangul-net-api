namespace HangulApi.Endpoints;

public static class ApiEndpoints
{
    public static RouteGroupBuilder MapApiEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/health");
        ConfigureTestEndpoint(group);
        return group;
    }

    private static void ConfigureTestEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/", (CancellationToken cancellationToken) =>
        {
            return Results.Ok(new { message = "API funciona" });
        })
        .WithName("Endpoint de prueba")
        .WithDescription("Verifica que el API está funcionando")
        .WithTags("API");
    }
}
