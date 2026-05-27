# 📊 Formato de la Tabla Juegos - Base de Datos

## Estructura de la Tabla

```sql
CREATE TABLE [dbo].[Juegos] (
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY DEFAULT (NEWID()),
	[Nombre] [nvarchar](200) NOT NULL,
	[Descripcion] [nvarchar](2000) NULL,
	[GeneroPrincipal] [nvarchar](100) NULL,
	[Tags] [nvarchar](1000) NULL,
	[FechaLanzamiento] [date] NULL,
	[Plataforma] [nvarchar](100) NULL,
	[ImagenPortadaUrl] [nvarchar](500) NULL
);
```

## Definición de Columnas

| Columna | Tipo de Dato | Tamaño | Requerido | Descripción |
|---------|-------------|--------|-----------|------------|
| `Id` | `uniqueidentifier` | - | ✅ Sí | Identificador único (GUID). PK. Auto-generado por defecto. |
| `Nombre` | `nvarchar` | 200 | ✅ Sí | Nombre del juego. Ej: "Elden Ring", "Dark Souls III" |
| `Descripcion` | `nvarchar` | 2000 | ❌ No | Descripción detallada del juego (hasta 2000 caracteres) |
| `GeneroPrincipal` | `nvarchar` | 100 | ❌ No | Género del juego. Ej: "RPG", "Acción", "Aventura" |
| `Tags` | `nvarchar` | 1000 | ❌ No | Etiquetas separadas por comas para búsqueda. Ej: "rpg,action,fantasy" |
| `FechaLanzamiento` | `date` | - | ❌ No | Fecha de lanzamiento del juego (YYYY-MM-DD) |
| `Plataforma` | `nvarchar` | 100 | ❌ No | Plataforma(s) donde está disponible. Ej: "PC, PS5, Xbox Series X" |
| `ImagenPortadaUrl` | `nvarchar` | 500 | ❌ No | URL de la imagen de portada/thumbnail del juego |

## Entidad en C# (Domain Model)

```csharp
namespace NuevoForo.Domain.Entities;

public class Juego
{
	public Guid Id { get; set; }
	public string Nombre { get; set; } = string.Empty;
	public string? Descripcion { get; set; }
	public string? GeneroPrincipal { get; set; }
	public string? Tags { get; set; }
	public DateOnly? FechaLanzamiento { get; set; }
	public string? Plataforma { get; set; }
	public string? ImagenPortadaUrl { get; set; }

	// Relaciones
	public ICollection<Resena> Resenas { get; set; } = new List<Resena>();
	public ICollection<ContenidoUgc> ContenidosUgc { get; set; } = new List<ContenidoUgc>();
}
```

## Configuración EF Core

```csharp
public class JuegoConfiguration : IEntityTypeConfiguration<Juego>
{
	public void Configure(EntityTypeBuilder<Juego> builder)
	{
		builder.ToTable("Juegos");
		builder.HasKey(j => j.Id);

		builder.Property(j => j.Nombre)
			.HasMaxLength(200)
			.IsRequired();

		builder.Property(j => j.Descripcion)
			.HasMaxLength(2000);

		builder.Property(j => j.GeneroPrincipal)
			.HasMaxLength(100);

		builder.Property(j => j.Tags)
			.HasMaxLength(1000);

		builder.Property(j => j.Plataforma)
			.HasMaxLength(100);

		builder.Property(j => j.ImagenPortadaUrl)
			.HasMaxLength(500);

		// Relaciones one-to-many
		builder.HasMany(j => j.Resenas)
			.WithOne(r => r.Juego)
			.HasForeignKey(r => r.JuegoId);

		builder.HasMany(j => j.ContenidosUgc)
			.WithOne(c => c.Juego)
			.HasForeignKey(c => c.JuegoId);
	}
}
```

## Relaciones

### 1. Juego → Reseñas (One-to-Many)
- Un juego puede tener múltiples reseñas
- FK en tabla `Resenas`: `JuegoId` (uniqueidentifier)
- Cascade delete: No especificado (por defecto Restrict)

### 2. Juego → ContenidoUGC (One-to-Many)
- Un juego puede tener múltiples contenidos UGC (guías, mods, parches)
- FK en tabla `ContenidosUgc`: `JuegoId` (uniqueidentifier)
- Cascade delete: No especificado (por defecto Restrict)

## Ejemplos de Datos

### Ejemplo 1: Elden Ring
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "nombre": "Elden Ring",
  "descripcion": "Un juego de rol de acción oscuro desarrollado por FromSoftware en colaboración con George R. R. Martin.",
  "generoPrincipal": "RPG",
  "tags": "rpg,acción,fantasy,souls-like,multijugador",
  "fechaLanzamiento": "2022-02-25",
  "plataforma": "PC, PlayStation 4, PlayStation 5, Xbox One, Xbox Series X/S",
  "imagenPortadaUrl": "https://example.com/elden-ring.jpg"
}
```

### Ejemplo 2: Dark Souls III
```json
{
  "id": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
  "nombre": "Dark Souls III",
  "descripcion": "El tercer juego de la saga Dark Souls de FromSoftware.",
  "generoPrincipal": "RPG",
  "tags": "rpg,acción,fantasy,souls-like",
  "fechaLanzamiento": "2016-04-12",
  "plataforma": "PC, PlayStation 4, Xbox One",
  "imagenPortadaUrl": "https://example.com/dark-souls-3.jpg"
}
```

## DTOs Relacionados

### JuegoResponse (Lista y Detalle)
```csharp
public sealed class JuegoResponse
{
	public Guid Id { get; set; }
	public string Nombre { get; set; } = string.Empty;
	public string? Descripcion { get; set; }
	public string? GeneroPrincipal { get; set; }
	public string? Tags { get; set; }
	public DateOnly? FechaLanzamiento { get; set; }
	public string? Plataforma { get; set; }
	public string? ImagenPortadaUrl { get; set; }
}
```

### GameSelectDto (Selector en UGC Upload)
```csharp
public sealed class GameSelectDto
{
	public Guid Id { get; set; }
	public string Nombre { get; set; } = string.Empty;
	public string? ImagenPortadaUrl { get; set; }
}
```

## Endpoints Disponibles

### 1. GET /games
**Descripción:** Lista paginada de juegos con filtros

**Parámetros:**
- `texto` (opcional): Buscar en nombre/descripción
- `genero` (opcional): Filtrar por género
- `tags` (opcional): Filtrar por tags
- `page` (default: 1): Número de página
- `pageSize` (default: 20, max: 100): Tamaño de página

**Respuesta:**
```json
{
  "page": 1,
  "pageSize": 20,
  "total": 150,
  "items": [
	{
	  "id": "550e8400-e29b-41d4-a716-446655440000",
	  "nombre": "Elden Ring",
	  "descripcion": "...",
	  "generoPrincipal": "RPG",
	  "tags": "rpg,acción,fantasy",
	  "fechaLanzamiento": "2022-02-25",
	  "plataforma": "PC, PS5, Xbox",
	  "imagenPortadaUrl": "https://..."
	}
  ]
}
```

### 2. GET /games/select
**Descripción:** Lista simplificada de todos los juegos (para selectors)

**Parámetros:** Ninguno

**Respuesta:**
```json
[
  {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"nombre": "Elden Ring",
	"imagenPortadaUrl": "https://..."
  },
  {
	"id": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
	"nombre": "Dark Souls III",
	"imagenPortadaUrl": "https://..."
  }
]
```

### 3. GET /games/{id}
**Descripción:** Obtiene detalles completos de un juego

**Parámetros:**
- `id` (Guid): Identificador del juego

**Respuesta:** 200 OK con JuegoResponse

### 4. POST /games (Solo Admin)
**Descripción:** Crea un nuevo juego

**Body:**
```json
{
  "nombre": "Nuevo Juego",
  "descripcion": "Descripción...",
  "generoPrincipal": "RPG",
  "tags": "rpg,acción",
  "fechaLanzamiento": "2024-01-15",
  "plataforma": "PC, PS5",
  "imagenPortadaUrl": "https://..."
}
```

### 5. PUT /games/{id} (Solo Admin)
**Descripción:** Actualiza un juego existente

**Parámetros:** Same as POST

### 6. DELETE /games/{id} (Solo Admin)
**Descripción:** Elimina un juego

**Respuesta:** 204 No Content

## Validaciones

### En Base de Datos
- `Nombre`: NOT NULL, longitud máxima 200
- `Id`: PRIMARY KEY, uniqueidentifier

### En Aplicación (C#)
- `Nombre`: [Required], [StringLength(200)]
- `Descripcion`: [StringLength(2000)]
- `GeneroPrincipal`: [StringLength(100)]
- `Tags`: [StringLength(1000)]
- `Plataforma`: [StringLength(100)]
- `ImagenPortadaUrl`: [StringLength(500)]

## Queries SQL Útiles

### Ver estructura actual
```sql
SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Juegos';
```

### Ver todos los juegos
```sql
SELECT Id, Nombre, GeneroPrincipal, FechaLanzamiento FROM Juegos ORDER BY Nombre;
```

### Buscar por nombre
```sql
SELECT * FROM Juegos WHERE Nombre LIKE '%Elden%';
```

### Contar juegos por género
```sql
SELECT GeneroPrincipal, COUNT(*) as Total FROM Juegos GROUP BY GeneroPrincipal;
```

### Ver juegos con contenido UGC
```sql
SELECT j.Nombre, COUNT(c.Id) as TotalUGC 
FROM Juegos j 
LEFT JOIN ContenidosUgc c ON j.Id = c.JuegoId 
GROUP BY j.Nombre 
HAVING COUNT(c.Id) > 0;
```

## Índices Recomendados

Para optimizar búsquedas:

```sql
-- Índice en Nombre para búsquedas LIKE
CREATE INDEX IX_Juegos_Nombre ON Juegos(Nombre);

-- Índice en GeneroPrincipal para filtros
CREATE INDEX IX_Juegos_GeneroPrincipal ON Juegos(GeneroPrincipal);

-- Índice en Tags para búsquedas
CREATE INDEX IX_Juegos_Tags ON Juegos(Tags);
```

## Migraciones EF Core

Si necesitas crear esta tabla desde cero:

```bash
# En carpeta raíz del proyecto
dotnet ef migrations add CreateJuegosTable -p src/NuevoForo.Infrastructure -s src/NuevoForo.Api

dotnet ef database update -p src/NuevoForo.Infrastructure -s src/NuevoForo.Api
```

## Notas Importantes

1. **FechaLanzamiento** es `DateOnly` en C#, se mapea a `DATE` en SQL (sin hora)
2. **ImagenPortadaUrl** es opcional - permite juegos sin imagen
3. **Relaciones:** Un juego puede tener múltiples reseñas y contenidos UGC
4. **Cascade Delete:** No está configurado, protege contra eliminaciones accidentales
5. **Tags:** Separados por comas, facilita búsquedas con LIKE
