using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuevoForo.Domain.Entities;

namespace NuevoForo.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Juego> Juegos => Set<Juego>();
    public DbSet<Resena> Resenas => Set<Resena>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<LikeResena> LikesResena => Set<LikeResena>();
    public DbSet<ContenidoUgc> ContenidosUgc => Set<ContenidoUgc>();
    public DbSet<LikeUgc> LikesUgc => Set<LikeUgc>();
    public DbSet<Reporte> Reportes => Set<Reporte>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<Donacion> Donaciones => Set<Donacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
