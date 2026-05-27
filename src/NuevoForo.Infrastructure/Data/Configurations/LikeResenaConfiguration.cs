using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data.Configurations;

public class LikeResenaConfiguration : IEntityTypeConfiguration<LikeResena>
{
    public void Configure(EntityTypeBuilder<LikeResena> builder)
    {
        builder.ToTable("LikesResena");
        builder.HasKey(l => l.Id);

        builder.HasIndex(l => new { l.UsuarioId, l.ResenaId })
            .IsUnique();
    }
}
