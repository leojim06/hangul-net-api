using HangulApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HangulApi.Configuration;

public class JamoConfiguration : IEntityTypeConfiguration<Jamo>
{
    public void Configure(EntityTypeBuilder<Jamo> builder)
    {
        builder.ToTable("Jamos");
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Character)
            .HasMaxLength(Jamo.CHARACTER_MAX_LENGTH)
            .IsRequired();
        builder.HasIndex(j => j.Character)
            .IsUnique(true);

        var jamoTypeValueConverter = new ValueConverter<JamoType, string>(
            to => to.Name,
            from => JamoType.FromName(from, true));
        builder.Property(j => j.Type)
            .HasConversion(jamoTypeValueConverter)
            .IsRequired();

        builder.Property(j => j.Pronunciation)
            .HasMaxLength(Jamo.PRONUNTIATION_MAX_LENGTH)
            .IsRequired();

        builder.Property(j => j.CharacterRomaji)
            .HasMaxLength(Jamo.CHARACTER_ROMAJI_MAX_LENGTH)
            .IsRequired();

        builder.Property(j => j.Name)
            .HasMaxLength(Jamo.NAME_ROMAJI_MAX_LENGTH)
            .IsRequired(false);

        builder.Property(j => j.NameRomaji)
            .HasMaxLength(Jamo.NAME_ROMAJI_MAX_LENGTH)
            .IsRequired(false);

        builder.Property(j => j.Description)
            .HasMaxLength(Jamo.DESCRIPTION_MAX_LENGTH)
            .IsRequired(false);

        builder
            .HasMany(j => j.Audios)
            .WithOne(a => a.Jamo)
            .HasForeignKey(a => a.JamoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
