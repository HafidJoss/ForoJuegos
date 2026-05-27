using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class ReporteConfiguration : IEntityTypeConfiguration<Reporte>
{
    public void Configure(EntityTypeBuilder<Reporte> builder)
    {
        builder.ToTable("Reportes");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Motivo)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(r => r.Evidencia)
            .HasMaxLength(2000);
    }
}
