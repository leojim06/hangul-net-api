using HangulApi.Data;
using HangulApi.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HangulApi.Features.General;

public static class HangulSummary
{
    public record Query : IRequest<Response>;

    public record Response(string Title, string Summary, IEnumerable<JamoCount> Content);

    public record JamoCount(string Title, int Count, string? Description);

    public class Handler(HangulDbContext _dbContext)
        : IRequestHandler<Query, Response>
    {
        private static readonly HashSet<JamoType> _vocalTypes = [JamoType.Vocal, JamoType.VocalDoble];
        private static readonly HashSet<JamoType> _consonantTypes = [JamoType.Consonante, JamoType.ConsonanteDoble, JamoType.ConsonanteDerivada];
        private static readonly HashSet<JamoType> _groupTypes = [JamoType.GrupoConsonantico];

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var allJamos = await _dbContext.Jamos.ToListAsync(cancellationToken);

            var total = allJamos.Count;
            var vocalesCount = allJamos.Count(j => _vocalTypes.Contains(j.Type));
            var consonantesCount = allJamos.Count(j => _consonantTypes.Contains(j.Type));
            var grupoConsonantico = allJamos.Count(j => _groupTypes.Contains(j.Type));

            var content = new List<JamoCount>()
            {
                new ("Total de caracteres", total, "Cantidad total de caracteres que hay registrados en la aplicación."),
                new ("Vocales", vocalesCount, "Caracteres que pueden ser de la categorías Vocal o Vocal doble."),
                new ("Consonantes", consonantesCount, "Caracteres que pueden ser de la categorías Consonante, Consonante doble o Consonante derivada."),
                new ("Grupo consonántico", grupoConsonantico, "Combinaciones de consonantes usadas para formar sílabas complejas.")
            };

            return new Response(
                "Hangul - Alfabeto Coreano",
                "Hangul es el alfabeto y sistema de escritura del idioma coreano. Es conocido por ser uno de los sistemas de escritura más lógicos y fáciles de aprender del mundo. \r\nEl nombre \"Hangul\" se traduce como \"gran escritura\" o \"escritura coreana\". Fue creado por el Rey Sejong El Grande en 1443. El hangul es el sistema de escritura oficial en Corea del Sur y Corea del Norte.",
                content);
        }
    }
}
