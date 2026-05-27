using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class LikeUgcConfiguration : IEntityTypeConfiguration<LikeUgc>
{
    public void Configure(EntityTypeBuilder<LikeUgc> builder)
    {
        builder.ToTable("LikesUgc");
        builder.HasKey(l => l.Id);

        builder.HasIndex(l => new { l.UsuarioId, l.UgcId })
            .IsUnique();

        builder.HasOne(l => l.Ugc)
            .WithMany()
            .HasForeignKey(l => l.UgcId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
