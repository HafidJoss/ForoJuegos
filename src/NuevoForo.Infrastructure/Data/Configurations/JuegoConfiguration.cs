using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class JuegoConfiguration : IEntityTypeConfiguration<Juego>
{
    public void Configure(EntityTypeBuilder<Juego> builder)
    {
        builder.ToTable("Juegos");
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(j => j.Descripcion)
            .HasMaxLength(2000);

        builder.Property(j => j.GeneroPrincipal)
            .HasMaxLength(100);

        builder.Property(j => j.Tags)
            .HasMaxLength(1000);

        builder.Property(j => j.Plataforma)
            .HasMaxLength(100);

        builder.Property(j => j.ImagenPortadaUrl)
            .HasMaxLength(500);

        builder.HasMany(j => j.Resenas)
            .WithOne(r => r.Juego)
            .HasForeignKey(r => r.JuegoId);

        builder.HasMany(j => j.ContenidosUgc)
            .WithOne(c => c.Juego)
            .HasForeignKey(c => c.JuegoId);
    }
}
