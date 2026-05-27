using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("Notificaciones");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Mensaje)
            .HasMaxLength(1000)
            .IsRequired();
    }
}
