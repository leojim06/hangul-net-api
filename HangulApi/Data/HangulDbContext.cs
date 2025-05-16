using HangulApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HangulApi.Data;

public class HangulDbContext(DbContextOptions<HangulDbContext> options) : DbContext(options)
{
    public DbSet<Jamo> Jamos { get; set; }
    public DbSet<Audio> Audios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
