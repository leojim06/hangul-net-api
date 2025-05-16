using FluentValidation;
using HangulApi.Entities;
using HangulApi.Features.Audios;
using HangulApi.Features.Jamos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulApi.Endpoints;

public static class JamoEndpoints
{
    public static RouteGroupBuilder MapJamoEndpoints(this IEndpointRouteBuilder routes)
    {
        // Crea un grupo de rutas bajo el perfil "/api/jamos"
        var group = routes.MapGroup("/api/v1/jamos");

        ConfigureCreateJamoEndpoint(group);
        ConfigureDeleteJamoEndpoint(group);
        ConfigureFindJamoEndpoint(group);
        ConfigureGetAllJamosEndpoint(group);
        ConfigureUpdateJamoEndpoint(group);
        //ConfigureUploadAudioEndpoint(group); 

        return group;
    }

    private static void ConfigureCreateJamoEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            ISender sender,
            CreateJamo.Command command,
            CancellationToken cancellationToken) =>
        {
            var jamo = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/v1/jamos/{jamo.Id}", jamo);
        })
        .WithName("Crear Jamo")
        .WithDescription("Crea un nuevo caracter del idioma Coreano (Hangul) en la base de datos")
        .WithTags("Jamos");
    }

    private static void ConfigureDeleteJamoEndpoint(RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new DeleteJamo.Command(id), cancellationToken);
            return response is null
                ? Results.NotFound(new { message = $"Jamo with id {id} not found" })
                : Results.NoContent();
        })
        .WithName("Eliminar Jamo")
        .WithDescription("Borra un caracter del idioma Coreano (Hangul) por su identificador en la base de datos")
        .WithTags("Jamos");
    }

    private static void ConfigureFindJamoEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/{id}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
        {
            var jamo = await sender.Send(new FindJamo.Query(id), cancellationToken);
            return jamo is null
                ? Results.NotFound(new { message = $"Jamo with id {id} not found" })
                : Results.Ok(jamo);
        })
        .WithName("Buscar un Jamo")
        .WithDescription("Busca un caracter del idioma Coreano (Hangul) por su identificador en la base de datos")
        .WithTags("Jamos");
    }

    private static void ConfigureGetAllJamosEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var jamos = await sender.Send(new GetAllJamos.Query(), cancellationToken);
            return Results.Ok(jamos);
        })
        .WithName("Lista todos los Jamos")
        .WithDescription("Lista todos los caracter del idioma Coreano (Hangul) en la base de datos")
        .WithTags("Jamos");
    }

    private static void ConfigureUpdateJamoEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", async (
            ISender sender,
            Guid id,
            UpdateJamoRequest request,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateJamo.Command(id, request.Character, request.Type, request.Pronunciation, request.CharacterRomaji,
                request.Name, request.NameRomaji, request.Description);
            var response = await sender.Send(command, cancellationToken);
            return response is null
                ? Results.NotFound(new { message = $"Jamo with id {command.Id} not found" })
                : Results.NoContent();
        })
        .WithName("Actualizar Jamo")
        .WithDescription("Actualiza un caracter del idioma Coreano (Hangul) por su identificador en la base de datos")
        .WithTags("Jamos");
    }

    private static void ConfigureUploadAudioEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/{guid:jamoId}/audios", async (
            ISender sender,
            Guid jamoId,
            IFormFile file,
            VoiceType voiceType,
            [FromForm] Guid? combinedWith,
            CancellationToken cancellationToken) =>
        {
            var command = new UploadAudio.Command(jamoId, file, voiceType, combinedWith);
            var audio = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/v1/jamos/{jamoId}/audios/{audio.Id}", audio);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .WithName("Subir audio de Jamo")
        .WithDescription("Sube un nuevo archivo de audio para un jamo y lo almacena en Azure Blob Storage")
        .WithTags("Jamos", "Audios");
    }

    private record UpdateJamoRequest(string Character, JamoType Type, string Pronunciation, string CharacterRomaji,
         string? Name, string? NameRomaji, string? Description);
}
