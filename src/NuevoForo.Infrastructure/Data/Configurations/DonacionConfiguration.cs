using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class DonacionConfiguration : IEntityTypeConfiguration<Donacion>
{
    public void Configure(EntityTypeBuilder<Donacion> builder)
    {
        builder.ToTable("Donaciones");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Monto)
            .HasColumnType("numeric(12,2)")
            .IsRequired();

        builder.Property(d => d.Moneda)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(d => d.ProveedorPago)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.TransaccionId)
            .HasMaxLength(200)
            .IsRequired();
    }
}
