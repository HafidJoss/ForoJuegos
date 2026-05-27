using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class ResenaConfiguration : IEntityTypeConfiguration<Resena>
{
    public void Configure(EntityTypeBuilder<Resena> builder)
    {
        builder.ToTable("Resenas");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Texto)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.HasIndex(r => new { r.UsuarioId, r.JuegoId })
            .IsUnique();

        builder.HasMany(r => r.Comentarios)
            .WithOne(c => c.Resena)
            .HasForeignKey(c => c.ResenaId);

        builder.HasMany(r => r.Likes)
            .WithOne(l => l.Resena)
            .HasForeignKey(l => l.ResenaId);
    }
}
