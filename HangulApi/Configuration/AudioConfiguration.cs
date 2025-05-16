using HangulApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HangulApi.Configuration;

public class AudioConfiguration : IEntityTypeConfiguration<Audio>
{
    public void Configure(EntityTypeBuilder<Audio> builder)
    {
        builder.ToTable("Audios");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.VoiceType)
            .HasConversion(v => v.Name, v => VoiceType.FromName(v, true));
    }
}
