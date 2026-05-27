using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(u => u.Nombre)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(u => u.Bio)
            .HasMaxLength(1000);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(u => u.Idioma)
            .HasMaxLength(10);

        builder.Property(u => u.UserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.UserName).IsUnique();

        builder.HasMany(u => u.Resenas)
            .WithOne(r => r.Usuario)
            .HasForeignKey(r => r.UsuarioId);

        builder.HasMany(u => u.Comentarios)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.UsuarioId);

        builder.HasMany(u => u.LikesResena)
            .WithOne(l => l.Usuario)
            .HasForeignKey(l => l.UsuarioId);

        builder.HasMany(u => u.ContenidosUgc)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.UsuarioId);

        builder.HasMany(u => u.ReportesRealizados)
            .WithOne(r => r.ReportadoPorUsuario)
            .HasForeignKey(r => r.ReportadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReportesModerados)
            .WithOne(r => r.Moderador)
            .HasForeignKey(r => r.ModeradorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.Notificaciones)
            .WithOne(n => n.Usuario)
            .HasForeignKey(n => n.UsuarioId);

        builder.HasMany(u => u.Donaciones)
            .WithOne(d => d.Usuario)
            .HasForeignKey(d => d.UsuarioId);
    }
}
