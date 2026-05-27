using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NuevoForo.Infrastructure.Data;

// Script para verificar datos de juegos importados
var connectionString = "Server=localhost;Port=5432;Database=NuevoForo_DB;User Id=postgres;Password=123456";
var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using (var context = new AppDbContext(optionsBuilder.Options))
{
    var juegos = context.Juegos.OrderBy(j => j.Nombre).ToList();

    Console.WriteLine($"✅ Total de juegos en BD: {juegos.Count()}");
    Console.WriteLine(new string('-', 80));

    foreach (var juego in juegos)
    {
        Console.WriteLine($"ID: {juego.Id:D}");
        Console.WriteLine($"Nombre: {juego.Nombre}");
        Console.WriteLine($"Género: {juego.GeneroPrincipal}");
        Console.WriteLine($"Plataforma: {juego.Plataforma}");
        Console.WriteLine($"Fecha Lanzamiento: {juego.FechaLanzamiento}");
        Console.WriteLine($"URL Imagen: {juego.ImagenPortadaUrl}");
        Console.WriteLine(new string('-', 80));
    }
}
