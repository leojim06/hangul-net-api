namespace HangulApi.Entities;

public class Jamo
{
    public const int CHARACTER_MAX_LENGTH = 1;
    public const int PRONUNTIATION_MAX_LENGTH = 20;
    public const int CHARACTER_ROMAJI_MAX_LENGTH = 20;
    public const int NAME_MAX_LENGTH = 10;
    public const int NAME_ROMAJI_MAX_LENGTH = 10;
    public const int DESCRIPTION_MAX_LENGTH = 120;
    public const int IMAGE_URL_MAX_LENGTH = 600;

    public Guid Id { get; set; }
    public required string Character { get; set; } = string.Empty;
    public required JamoType Type { get; set; }
    public required string Pronunciation { get; set; } = string.Empty;
    public required string CharacterRomaji { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public string? NameRomaji { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
    public ICollection<Audio> Audios { get; set; } = [];
}