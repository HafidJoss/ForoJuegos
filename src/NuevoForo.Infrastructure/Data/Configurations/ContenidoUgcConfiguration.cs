using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class ContenidoUgcConfiguration : IEntityTypeConfiguration<ContenidoUgc>
{
    public void Configure(EntityTypeBuilder<ContenidoUgc> builder)
    {
        builder.ToTable("ContenidosUgc");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Titulo)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasMaxLength(4000);

        builder.Property(c => c.ArchivoUrl)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.ArchivoNombre)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.ArchivoHash)
            .HasMaxLength(128);

        builder.Property(c => c.FotoPath)
            .HasMaxLength(1000);

        builder.Property(c => c.FotoNombre)
            .HasMaxLength(255);

        builder.Property(c => c.Tags)
            .HasMaxLength(1000);

        // Foreign key a Juego (opcional)
        builder.HasOne(c => c.Juego)
            .WithMany(j => j.ContenidosUgc)
            .HasForeignKey(c => c.JuegoId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Foreign key a Usuario
        builder.HasOne(c => c.Usuario)
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
