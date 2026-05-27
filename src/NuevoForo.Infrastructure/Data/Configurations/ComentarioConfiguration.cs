using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class ComentarioConfiguration : IEntityTypeConfiguration<Comentario>
{
    public void Configure(EntityTypeBuilder<Comentario> builder)
    {
        builder.ToTable("Comentarios");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Texto)
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasOne(c => c.Resena)
            .WithMany(r => r.Comentarios)
            .HasForeignKey(c => c.ResenaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Ugc)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UgcId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
