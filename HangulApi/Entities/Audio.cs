namespace HangulApi.Entities;

public class Audio
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required VoiceType VoiceType { get; set; }
    // Opcional: Jamo con el que se combina la pronunciación (solo para voz combinada)
    public Guid? CombinedWith { get; set; }
    // Foreign Key hacia Jamo principal
    public Guid JamoId { get; set; }
    // Propiedad de navegación
    public Jamo Jamo { get; set; } = default!;
}
