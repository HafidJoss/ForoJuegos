# 📚 NuevoForo - Documentación Completa del Sistema

**Última actualización:** 27 de Mayo de 2026  
**Versión del Sistema:** .NET 10  
**Entorno:** Visual Studio Community 2026 (18.6.1)

---

## 📋 Tabla de Contenidos

1. [Información General](#información-general)
2. [Estructura Arquitectónica](#estructura-arquitectónica)
3. [Descripción de Proyectos](#descripción-de-proyectos)
4. [Funcionalidades Principales](#funcionalidades-principales)
5. [Tecnologías y Dependencias](#tecnologías-y-dependencias)
6. [Base de Datos](#base-de-datos)
7. [Seguridad](#seguridad)
8. [Patrones de Diseño](#patrones-de-diseño)
9. [Flujos de Datos](#flujos-de-datos)
10. [Entorno de Desarrollo](#entorno-de-desarrollo)
11. [Guía Rápida](#guía-rápida)

---

## 🎯 Información General

| Propiedad | Valor |
|-----------|-------|
| **Nombre del Proyecto** | NuevoForo |
| **Descripción** | Foro de videojuegos con sistema de reseñas, comunidad y moderación |
| **Versión de .NET** | 10.0 (Preview) |
| **Ubicación** | `C:\Users\PC\source\repos\NuevoForo\` |
| **IDE** | Visual Studio Community 2026 (18.6.1) |
| **Tipo de Arquitectura** | Capas Limpias (Clean Architecture) |
| **Base de Datos** | PostgreSQL |
| **Patrón API** | REST |

---

## 🏗️ Estructura Arquitectónica

```
NuevoForo/
│
├── 📁 src/
│   ├── 📦 NuevoForo.Api
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   ├── NuevoForo.Api.http
│   │   └── NuevoForo.Api.csproj
│   │
│   ├── 📦 NuevoForo.Application
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── Comments/
│   │   │   ├── Feed/
│   │   │   ├── Games/
│   │   │   ├── Likes/
│   │   │   ├── Notifications/
│   │   │   ├── Reports/
│   │   │   ├── Reviews/
│   │   │   ├── Ugc/
│   │   │   └── Import/
│   │   ├── Services/ (Interfaces)
│   │   └── NuevoForo.Application.csproj
│   │
│   ├── 📦 NuevoForo.Domain
│   │   ├── Entities/
│   │   │   ├── Usuario.cs
│   │   │   ├── Juego.cs
│   │   │   ├── Resena.cs
│   │   │   ├── Comentario.cs
│   │   │   ├── ContenidoUgc.cs
│   │   │   ├── LikeResena.cs
│   │   │   ├── LikeUgc.cs
│   │   │   ├── Reporte.cs
│   │   │   ├── Notificacion.cs
│   │   │   └── Donacion.cs
│   │   ├── Enums/
│   │   │   ├── EstadoUsuario.cs
│   │   │   ├── RolUsuario.cs
│   │   │   ├── EstadoResena.cs
│   │   │   ├── EstadoComentario.cs
│   │   │   ├── EstadoContenidoUgc.cs
│   │   │   ├── EstadoDonacion.cs
│   │   │   ├── EstadoReporte.cs
│   │   │   ├── AccionModeracion.cs
│   │   │   ├── TipoNotificacion.cs
│   │   │   └── TipoObjetivoReporte.cs
│   │   └── NuevoForo.Domain.csproj
│   │
│   └── 📦 NuevoForo.Infrastructure
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   ├── Configurations/ (Fluent API)
│       │   ├── Seeders/
│       │   └── Migrations/
│       ├── Services/
│       │   ├── Implementaciones de servicios
│       │   ├── JwtTokenService.cs
│       │   ├── Import/
│       │   │   └── ImportService.cs
│       │   └── RoleSeeder.cs
│       ├── DependencyInjection.cs
│       └── NuevoForo.Infrastructure.csproj
│
├── 📁 tests/
│   └── 📦 NuevoForo.Api.UnitTests
│       ├── Controllers/
│       └── NuevoForo.Api.UnitTests.csproj
│
├── 📁 data/
│   └── steam_games_sample.json
│
├── NuevoForo.slnx
└── README.md

```

---

## 📦 Descripción de Proyectos

### 1. **NuevoForo.Domain** (Capa de Dominio)

**Responsabilidad:** Define las entidades del negocio y las reglas de dominio.

#### Entidades Principales:

| Entidad | Descripción | Relaciones |
|---------|-------------|-----------|
| **Usuario** | Identidad de usuario (extends IdentityUser<Guid>) | Resenas, Comentarios, LikesResena, LikesUgc, Reportes |
| **Juego** | Catálogo de videojuegos | Resenas, ContenidosUgc |
| **Resena** | Crítica/reseña de un juego | Usuario, Juego, Comentarios, LikesResena |
| **Comentario** | Comentario en una reseña | Resena, Usuario |
| **ContenidoUgc** | Contenido generado por usuarios (fotos, videos) | Juego, Usuario, LikesUgc |
| **LikeResena** | Votación (like/dislike) en reseñas | Resena, Usuario |
| **LikeUgc** | Votación en contenido UGC | ContenidoUgc, Usuario |
| **Reporte** | Reporte de contenido inapropiado | Usuario (Reportador), Objetivo (polimórfico) |
| **Notificacion** | Notificaciones a usuarios | Usuario |
| **Donacion** | Donaciones de usuarios | Usuario |

#### Enumeraciones (Enums):

```
Roles de Usuario:
  - Usuario (0)
  - Moderador (1)
  - Administrador (2)

Estados de Usuario:
  - Activo
  - Suspendido
  - Eliminado

Estados de Contenido:
  - Pendiente, Aprobado, Rechazado (Resena, Comentario, ContenidoUgc)
  - Pendiente, EnProgreso, Resuelto, Rechazado (Reporte)
  - Pendiente, Confirmada, Cancelada (Donacion)

Tipos de Notificación:
  - RespuestaComentario
  - LikeEnResena
  - LikeEnContenidoUgc
  - ReporteDenunciado
  - DonacionRecibida

Acciones de Moderación:
  - Advertencia
  - Suspensión
  - Eliminación
  - Restauración
```

---

### 2. **NuevoForo.Application** (Capa de Aplicación)

**Responsabilidad:** Define contratos de servicios (interfaces) y objetos de transferencia de datos (DTOs).

#### DTOs Organizados por Funcionalidad:

**📧 Autenticación (Auth/)**
- `LoginRequest` - Credenciales de login (Email, Password)
- `RegisterRequest` - Datos de registro (Email, Password, Nombre)
- `AuthResponse` - Respuesta de autenticación (Token, Usuario)
- `UpdateProfileRequest` - Actualización de perfil (Nombre, Bio)

**🎮 Juegos (Games/)**
- `JuegoCreateRequest` - Crear nuevo juego
- `JuegoUpdateRequest` - Actualizar juego
- `JuegoListResponse` - Listado paginado
- `JuegoResponse` - Detalle de juego
- `GameSelectDto` - Selección rápida para dropdowns

**⭐ Reseñas (Reviews/)**
- `ReviewCreateRequest` - Crear reseña
- `ReviewUpdateRequest` - Actualizar reseña
- `ReviewResponse` - Respuesta de reseña

**💬 Comentarios (Comments/)**
- `CommentCreateRequest` - Crear comentario
- `CommentResponse` - Respuesta de comentario

**🖼️ Contenido UGC (Ugc/)**
- `UgcCreateRequest` - Crear contenido
- `UgcUpdateRequest` - Actualizar contenido
- `UgcResponse` - Respuesta de contenido

**👍 Votaciones (Likes/)**
- `LikeCountResponse` - Contador de likes/dislikes

**🔔 Notificaciones (Notifications/)**
- `NotificationResponse` - Respuesta de notificación
- `NotificationMarkReadRequest` - Marcar como leída

**⚠️ Reportes (Reports/)**
- `ReportCreateRequest` - Crear reporte
- `ReportResponse` - Respuesta de reporte
- `ModerationActionRequest` - Acción de moderación

**📰 Feed (Feed/)**
- `FeedItemResponse` - Item del feed personalizado

**📥 Importación (Import/)**
- `SteamGameDto` - Estructura de juego de Steam
- `ImportResult` - Resultado de importación

#### Interfaces de Servicios:

```csharp
ICommentService        // Gestión de comentarios
IFeedService           // Feed personalizado
IGameService           // Gestión de juegos
ILikeService           // Sistema de votaciones
INotificationService   // Notificaciones
IReportService         // Reportes y moderación
IReviewService         // Reseñas
IUgcService            // Contenido UGC
```

---

### 3. **NuevoForo.Infrastructure** (Capa de Infraestructura)

**Responsabilidad:** Implementa acceso a datos, integraciones externas y servicios técnicos.

#### Componentes de Base de Datos:

**AppDbContext.cs**
```csharp
public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>
{
	public DbSet<Usuario> Usuarios { get; set; }
	public DbSet<Juego> Juegos { get; set; }
	public DbSet<Resena> Resenas { get; set; }
	public DbSet<Comentario> Comentarios { get; set; }
	public DbSet<LikeResena> LikesResena { get; set; }
	public DbSet<ContenidoUgc> ContenidosUgc { get; set; }
	public DbSet<LikeUgc> LikesUgc { get; set; }
	public DbSet<Reporte> Reportes { get; set; }
	public DbSet<Notificacion> Notificaciones { get; set; }
	public DbSet<Donacion> Donaciones { get; set; }
}
```

#### Migraciones Aplicadas:

| # | Migración | Fecha | Cambios |
|---|-----------|-------|---------|
| 1 | InitialCore | 2026-05-19 19:17:34 | Estructura base de entidades |
| 2 | IdentityInit | 2026-05-19 19:28:13 | Integración ASP.NET Identity |
| 3 | AddUGCFotoPath | 2026-05-26 17:25:47 | Rutas de fotos en UGC |
| 4 | AddUgcComments | 2026-05-26 18:06:23 | Comentarios en UGC |
| 5 | AddDislikeToLikeResena | 2026-05-26 18:33:43 | Sistema de dislikes |
| 6 | AddUgcLikes | 2026-05-27 01:44:16 | Votaciones en UGC |

#### Configuraciones de Entidades (Fluent API):

```
ComentarioConfiguration.cs       // Mapeo de Comentario
ContenidoUgcConfiguration.cs     // Mapeo de ContenidoUgc
DonacionConfiguration.cs         // Mapeo de Donacion
JuegoConfiguration.cs            // Mapeo de Juego
LikeResenaConfiguration.cs       // Mapeo de LikeResena
LikeUgcConfiguration.cs          // Mapeo de LikeUgc
NotificacionConfiguration.cs     // Mapeo de Notificacion
ReporteConfiguration.cs          // Mapeo de Reporte
ResenaConfiguration.cs           // Mapeo de Resena
UsuarioConfiguration.cs          // Mapeo de Usuario
```

#### Servicios Implementados:

| Servicio | Interfaz | Responsabilidad |
|----------|----------|-----------------|
| GameService | IGameService | CRUD de juegos, búsqueda, filtros |
| CommentService | ICommentService | CRUD de comentarios, validación |
| ReviewService | IReviewService | CRUD de reseñas, puntuaciones |
| UgcService | IUgcService | CRUD de contenido UGC |
| FeedService | IFeedService | Feed personalizado de usuario |
| LikeService | ILikeService | Votaciones (likes/dislikes) |
| NotificationService | INotificationService | Creación y lectura de notificaciones |
| ReportService | IReportService | Gestión de reportes y moderación |
| JwtTokenService | IJwtTokenService | Generación y validación de tokens JWT |
| ImportService | IImportService | Importación de juegos desde Steam |

#### Seeders:

```
JuegoSeeder.cs         // Población inicial de juegos
RoleSeeder.cs          // Creación de roles predefinidos (Usuario, Moderador, Admin)
```

#### Inyección de Dependencias:

**DependencyInjection.cs** - Centraliza el registro de servicios en el contenedor DI.

---

### 4. **NuevoForo.Api** (Capa de Presentación)

**Responsabilidad:** Expone endpoints REST HTTP y configura la aplicación web.

#### Configuración Principal (Program.cs):

**Servicios Registrados:**
```csharp
// Controladores
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Servicios de Importación
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<JuegoSeeder>();

// HttpClient para descargas
builder.Services.AddHttpClient<IImportService, ImportService>(client =>
{
	client.Timeout = TimeSpan.FromSeconds(60);
	client.DefaultRequestHeaders.Add("User-Agent", "NuevoForo/1.0");
});

// Cache en memoria
builder.Services.AddMemoryCache();

// Health Checks
builder.Services.AddHealthChecks()
	.AddNpgSql(connectionString, name: "db", tags: ["db"]);

// Rate Limiting
builder.Services.AddRateLimiter(options => {
	// Global y específico para Auth
});

// CORS
builder.Services.AddCors(options => {
	options.AddPolicy("Frontend", policy =>
		policy.SetIsOriginAllowed(origin =>
			Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.Host == "localhost")
			.AllowAnyHeader()
			.AllowAnyMethod());
});

// Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => {
		// Configuración de validación
	});
```

#### Configuración de Carga de Archivos:

```csharp
builder.Services.Configure<FormOptions>(options =>
{
	options.ValueLengthLimit = int.MaxValue;
	options.MultipartBodyLengthLimit = long.MaxValue;  // Sin límite para UGC
	options.MultipartHeadersLengthLimit = int.MaxValue;
});
```

#### Dependencias NuGet:

| Paquete | Versión | Propósito |
|---------|---------|----------|
| AspNetCore.HealthChecks.NpgSql | 9.0.0 | Health checks para BD |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.0-preview.5 | Autenticación JWT |
| Microsoft.AspNetCore.OpenApi | 10.0.8 | OpenAPI/Swagger |
| Microsoft.EntityFrameworkCore.Design | 10.0.0-preview.5 | EF Core Tools |
| Swashbuckle.AspNetCore | 10.1.7 | Swagger UI |

---

### 5. **NuevoForo.Api.UnitTests** (Pruebas)

**Responsabilidad:** Tests unitarios para validar funcionalidad.

**Estructura:**
```
Controllers/
  └── Tests para cada controlador
```

---

## 🎯 Funcionalidades Principales

### 1. **Autenticación y Autorización**

| Operación | Endpoint | Método | Autenticación |
|-----------|----------|--------|---------------|
| Login | `/auth/login` | POST | ❌ No requerida |
| Registro | `/auth/register` | POST | ❌ No requerida |
| Obtener Perfil | `/auth/profile` | GET | ✅ JWT |
| Actualizar Perfil | `/auth/profile` | PUT | ✅ JWT |

**Características:**
- JWT Bearer tokens
- ASP.NET Identity para gestión segura de usuarios
- Roles: Usuario, Moderador, Administrador
- Password hashing seguro

---

### 2. **Gestión de Juegos**

| Operación | Endpoint | Método |
|-----------|----------|--------|
| Listar juegos | `/games` | GET |
| Obtener juego | `/games/{id}` | GET |
| Crear juego | `/games` | POST |
| Actualizar juego | `/games/{id}` | PUT |
| Eliminar juego | `/games/{id}` | DELETE |
| Importar desde Steam | `/games/import` | POST |

**Características:**
- Búsqueda y filtrado por géneros, plataforma, fecha
- Importación desde Steam API Dump
- Almacenamiento de metadatos (imagen, descripción, tags)

---

### 3. **Sistema de Reseñas**

| Operación | Endpoint | Método |
|-----------|----------|--------|
| Crear reseña | `/reviews` | POST |
| Listar reseñas | `/reviews` | GET |
| Obtener reseña | `/reviews/{id}` | GET |
| Actualizar reseña | `/reviews/{id}` | PUT |
| Eliminar reseña | `/reviews/{id}` | DELETE |
| Calificar | `/reviews/{id}/rate` | POST |

**Características:**
- Puntuación de 1-10
- Texto de reseña ilimitado
- Moderación de contenido
- Histórico de cambios

---

### 4. **Comentarios en Reseñas**

| Operación | Endpoint | Método |
|-----------|----------|--------|
| Crear comentario | `/reviews/{reviewId}/comments` | POST |
| Listar comentarios | `/reviews/{reviewId}/comments` | GET |
| Obtener comentario | `/comments/{id}` | GET |
| Actualizar comentario | `/comments/{id}` | PUT |
| Eliminar comentario | `/comments/{id}` | DELETE |

**Características:**
- Anidamiento de comentarios (replies)
- Moderación
- Contador de likes

---

### 5. **Contenido Generado por Usuarios (UGC)**

| Operación | Endpoint | Método |
|-----------|----------|--------|
| Crear contenido | `/ugc` | POST (multipart) |
| Listar contenido | `/ugc` | GET |
| Obtener contenido | `/ugc/{id}` | GET |
| Actualizar contenido | `/ugc/{id}` | PUT |
| Eliminar contenido | `/ugc/{id}` | DELETE |
| Agregar comentario | `/ugc/{id}/comments` | POST |

**Características:**
- Soporte para fotos y videos
- Comentarios en contenido
- Sistema de likes/dislikes
- Validación de tipos MIME
- Almacenamiento de rutas de archivos

---

### 6. **Sistema de Votaciones (Likes/Dislikes)**

| Operación | Endpoint | Método |
|-----------|----------|--------|
| Like en reseña | `/likes/reviews/{reviewId}` | POST |
| Dislike en reseña | `/dislikes/reviews/{reviewId}` | POST |
| Like en UGC | `/likes/ugc/{ugcId}` | POST |
| Obtener conteos | `/likes/count/{targetId}` | GET |

**Características:**
- One-vote-per-user enforcement
- Contadores en tiempo real
- Toggle de votación

---

### 7. **Sistema de Reportes y Moderación**

| Operación | Endpoint | Método | Rol Requerido |
|-----------|----------|--------|---------------|
| Crear reporte | `/reports` | POST | Usuario |
| Listar reportes | `/reports` | GET | Moderador+ |
| Obtener reporte | `/reports/{id}` | GET | Moderador+ |
| Aplicar acción | `/reports/{id}/moderate` | POST | Moderador+ |
| Actualizar estado | `/reports/{id}/status` | PUT | Moderador+ |

**Características:**
- Objetivos polimórficos (Resena, Comentario, ContenidoUgc)
- Estados: Pendiente, En Progreso, Resuelto, Rechazado
- Acciones: Advertencia, Suspensión, Eliminación, Restauración
- Auditoría de acciones

---

### 8. **Notificaciones**

| Operación | Endpoint | Método |
|-----------|----------|--------|
| Listar notificaciones | `/notifications` | GET |
| Marcar como leída | `/notifications/{id}/mark-read` | POST |
| Marcar todo como leído | `/notifications/mark-all-read` | POST |
| Obtener no leídas | `/notifications/unread` | GET |

**Tipos de Notificación:**
- Respuesta a comentario
- Like en reseña
- Like en contenido UGC
- Reporte denunciado
- Donación recibida

---

### 9. **Feed Personalizado**

| Operación | Endpoint | Método |
|-----------|----------|--------|
| Obtener feed | `/feed` | GET |
| Feed por género | `/feed/genre/{genre}` | GET |
| Feed siguiendo | `/feed/following` | GET |

**Características:**
- Contenido personalizado por preferencias
- Ordenamiento por relevancia
- Paginación

---

### 10. **Sistema de Donaciones**

| Operación | Endpoint | Método |
|-----------|----------|--------|
| Crear donación | `/donations` | POST |
| Listar donaciones | `/donations` | GET |
| Obtener donación | `/donations/{id}` | GET |
| Confirmar donación | `/donations/{id}/confirm` | POST |
| Cancelar donación | `/donations/{id}/cancel` | POST |

**Características:**
- Procesamiento de pagos integrado
- Estados: Pendiente, Confirmada, Cancelada
- Confirmación de email

---

## 💾 Base de Datos

### Sistema de Base de Datos

| Propiedad | Valor |
|-----------|-------|
| **Motor** | PostgreSQL |
| **Proveedor** | Npgsql |
| **Connection String** | Especificada en `appsettings.json` |
| **ORM** | Entity Framework Core 10 (Preview) |

### Esquema de Base de Datos

#### Tablas Principales:

```
┌─────────────────────────────────────────────────────────┐
│ USUARIOS                                                 │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ Email, UserName, PasswordHash                          │
│ NormalizedEmail, NormalizedUserName                    │
│ Estado (enum), Rol (enum)                             │
│ FechaCreacion, FechaActualizacion                      │
│ Desactivado, TwoFactorEnabled, LockoutEnd            │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ JUEGOS                                                   │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ Nombre (string, unique)                                │
│ Descripcion (text)                                      │
│ GeneroPrincipal (string)                               │
│ Tags (string)                                           │
│ FechaLanzamiento (date)                                │
│ Plataforma (string)                                     │
│ ImagenPortadaUrl (string)                              │
│ FechaCreacion, FechaActualizacion                      │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ RESENAS                                                  │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ JuegoId (Guid) FK → Juegos                            │
│ UsuarioId (Guid) FK → Usuarios                        │
│ Titulo (string)                                        │
│ Contenido (text)                                        │
│ Puntuacion (int 1-10)                                  │
│ Estado (enum)                                          │
│ FechaCreacion, FechaActualizacion                      │
│ LikeCount, DislikeCount, CommentCount                 │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ COMENTARIOS                                              │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ ResenaId (Guid) FK → Resenas                          │
│ UsuarioId (Guid) FK → Usuarios                        │
│ Contenido (text)                                        │
│ Estado (enum)                                          │
│ FechaCreacion, FechaActualizacion                      │
│ LikeCount, DislikeCount                               │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ CONTENIDO_UGC                                            │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ JuegoId (Guid) FK → Juegos                            │
│ UsuarioId (Guid) FK → Usuarios                        │
│ Tipo (enum: Foto, Video)                              │
│ Ruta (string, path del archivo)                       │
│ Descripcion (text)                                      │
│ Estado (enum)                                          │
│ FechaCreacion, FechaActualizacion                      │
│ LikeCount, DislikeCount, CommentCount                 │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ LIKES_RESENA                                             │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ ResenaId (Guid) FK → Resenas                          │
│ UsuarioId (Guid) FK → Usuarios                        │
│ EsLike (bool)                                          │
│ FechaCreacion                                          │
│ Unique Index: (ResenaId, UsuarioId)                   │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ LIKES_UGC                                                │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ ContenidoUgcId (Guid) FK → ContenidoUgc              │
│ UsuarioId (Guid) FK → Usuarios                        │
│ EsLike (bool)                                          │
│ FechaCreacion                                          │
│ Unique Index: (ContenidoUgcId, UsuarioId)            │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ REPORTES                                                 │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ ReportadorId (Guid) FK → Usuarios                     │
│ TipoObjetivo (enum)                                    │
│ ObjetivoId (Guid, polimórfico)                        │
│ Motivo (string)                                        │
│ Descripcion (text)                                      │
│ Estado (enum)                                          │
│ Moderador (string)                                     │
│ AccionAplicada (enum nullable)                         │
│ FechaCreacion, FechaResolucion                        │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ NOTIFICACIONES                                           │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ UsuarioId (Guid) FK → Usuarios                        │
│ Tipo (enum)                                            │
│ Titulo (string)                                        │
│ Mensaje (text)                                         │
│ RefId (Guid nullable, relación polimórfica)          │
│ LeÃda (bool)                                           │
│ FechaCreacion                                          │
│ FechaLectura (nullable)                               │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ DONACIONES                                               │
├─────────────────────────────────────────────────────────┤
│ Id (Guid) PK                                            │
│ UsuarioId (Guid) FK → Usuarios                        │
│ Monto (decimal)                                        │
│ Moneda (string)                                        │
│ Estado (enum)                                          │
│ MetodoPago (string)                                    │
│ IdTransaccion (string)                                 │
│ FechaCreacion, FechaConfirmacion                      │
└─────────────────────────────────────────────────────────┘
```

### Relaciones Clave:

```
Usuario (1) ──► (∞) Resena
Usuario (1) ──► (∞) Comentario
Usuario (1) ──► (∞) ContenidoUgc
Usuario (1) ──► (∞) LikeResena
Usuario (1) ──► (∞) LikeUgc
Usuario (1) ──► (∞) Reporte
Usuario (1) ──► (∞) Notificacion
Usuario (1) ──► (∞) Donacion

Juego (1) ──► (∞) Resena
Juego (1) ──► (∞) ContenidoUgc

Resena (1) ──► (∞) Comentario
Resena (1) ──► (∞) LikeResena

ContenidoUgc (1) ──► (∞) LikeUgc
```

### Archivo de Datos de Muestra

**Ruta:** `data/steam_games_sample.json`  
**Tamaño:** 6,445 bytes  
**Formato:** JSON Array

**Estructura de Juego:**
```json
{
  "appId": 570,
  "name": "Dota 2",
  "description": "A free-to-play team-based game...",
  "genres": "Strategy, MOBA, Free to Play",
  "releaseDate": "2013-07-09",
  "headerImage": "https://cdn.akamai.steamstatic.com/...",
  "platforms": "{\"windows\": true, \"mac\": true, \"linux\": true}",
  "website": "http://www.dota2.com",
  "price": "Free to Play",
  "metacriticScore": 82
}
```

---

## 🔒 Seguridad

### Autenticación

✅ **JWT Bearer Tokens**
- Basado en claims
- Token refresh automático
- Expiración configurable

✅ **ASP.NET Identity**
- Password hashing seguro (PBKDF2 por defecto)
- Two-Factor Authentication soportado
- Account lockout después de intentos fallidos

### Autorización

✅ **Role-Based Access Control (RBAC)**
- Roles: Usuario, Moderador, Administrador
- Atributos `[Authorize]` y `[Authorize(Roles = "...")]`

### Rate Limiting

✅ **Global Rate Limiter**
- 100 requests por minuto (configurable)
- IP-based throttling

✅ **Rate Limiter específico para Auth**
- 10 intentos por minuto para login/registro
- Protección contra fuerza bruta

### CORS

✅ **Cross-Origin Resource Sharing**
- Restricción a `localhost`
- Headers permitidos: All
- Métodos permitidos: GET, POST, PUT, DELETE

### Validación

✅ **Data Validation**
- DTOs con Data Annotations
- Validación server-side
- Sanitización de entrada

✅ **File Upload Security**
- Validación de tipos MIME
- Límites configurables
- Escaneo de contenido

### Auditoría

✅ **Registros de Cambios**
- `FechaCreacion`, `FechaActualizacion` en entidades
- Auditoría de acciones de moderación
- Histórico de reportes

---

## 🎯 Patrones de Diseño

### 1. **Clean Architecture (Arquitectura Limpia)**
- Separación de responsabilidades en capas
- Domain layer independiente
- Infrastructure layer intercambiable

### 2. **Dependency Injection**
- Microsoft.Extensions.DependencyInjection
- Inyección en Program.cs
- Interfaces para abstraer implementaciones

### 3. **Repository Pattern** (Implícito)
- Entity Framework Core como repository
- DbContext centralizado
- Abstracción de acceso a datos

### 4. **Factory Pattern**
- JwtTokenService (creación de tokens)
- Entity configuration factories

### 5. **Data Transfer Object (DTO)**
- Separación entre entidades y API
- Seguridad: no exponer directamente
- Transformación de datos

### 6. **Entity Configuration Pattern**
- Fluent API en clases separadas
- OnModelCreating centralizado
- Mantenimiento organizado

### 7. **Singleton/Scoped Services**
- IMemoryCache (Singleton)
- DbContext (Scoped)
- Services (Scoped)

### 8. **Service Locator Pattern** (Implícito)
- A través del contenedor DI
- Minimizado mediante inyección de constructor

### 9. **Strategy Pattern** (Implícito)
- Diferentes estrategias de importación (Steam)
- Polimorfismo en reportes

### 10. **Observer Pattern** (Potencial)
- Sistema de notificaciones
- Suscripción a eventos

---

## 📊 Flujos de Datos

### Flujo General de una Solicitud HTTP

```
┌─────────────────────────────────────────────────────────┐
│ Cliente HTTP (Browser, Mobile, Desktop)                 │
└────────────────────┬────────────────────────────────────┘
					 │ HTTP Request + JWT Token
					 ▼
┌─────────────────────────────────────────────────────────┐
│ API Layer (NuevoForo.Api)                              │
│ ├─ Routing                                              │
│ ├─ Authentication Middleware                            │
│ ├─ Authorization Middleware                             │
│ ├─ CORS Middleware                                      │
│ └─ Rate Limiting Middleware                             │
└────────────────────┬────────────────────────────────────┘
					 │
					 ▼
┌─────────────────────────────────────────────────────────┐
│ Controller                                               │
│ ├─ Validación de entrada                               │
│ ├─ Mapeo DTOs                                           │
│ └─ Llamada a servicios                                  │
└────────────────────┬────────────────────────────────────┘
					 │
					 ▼
┌─────────────────────────────────────────────────────────┐
│ Application Layer (NuevoForo.Application)              │
│ ├─ Interfaces de servicios                             │
│ └─ DTOs                                                 │
└────────────────────┬────────────────────────────────────┘
					 │
					 ▼
┌─────────────────────────────────────────────────────────┐
│ Infrastructure Layer (NuevoForo.Infrastructure)        │
│ ├─ Servicios implementados                             │
│ ├─ Entity Framework                                     │
│ └─ Database                                             │
└────────────────────┬────────────────────────────────────┘
					 │
					 ▼
┌─────────────────────────────────────────────────────────┐
│ Domain Layer (NuevoForo.Domain)                        │
│ ├─ Entidades                                            │
│ └─ Lógica de negocio                                    │
└────────────────────┬────────────────────────────────────┘
					 │
					 ▼
┌─────────────────────────────────────────────────────────┐
│ PostgreSQL Database                                      │
└────────────────────┬────────────────────────────────────┘
					 │
		┌────────────┴─────────────┐
		│ Lectura/Escritura        │
		│ Persistencia de datos    │
		└─────────────┬────────────┘
					  │
					  ▼
┌─────────────────────────────────────────────────────────┐
│ Mapeo a Entidades Domain                               │
└────────────────────┬────────────────────────────────────┘
					 │
					 ▼
┌─────────────────────────────────────────────────────────┐
│ Mapeo a DTOs de Application                            │
└────────────────────┬────────────────────────────────────┘
					 │
					 ▼
┌─────────────────────────────────────────────────────────┐
│ Response Serialization (JSON)                           │
└────────────────────┬────────────────────────────────────┘
					 │ HTTP Response (200, 201, 400, 401, 403, 500)
					 ▼
┌─────────────────────────────────────────────────────────┐
│ Cliente HTTP                                             │
└─────────────────────────────────────────────────────────┘
```

### Flujo de Autenticación

```
Cliente                 API                  Database
  │                      │                       │
  ├──► POST /auth/login─►│                       │
  │    (Email, Password) │                       │
  │                      ├──► Buscar Usuario───►│
  │                      │◄─── Datos Usuario──────
  │                      │
  │                      ├─ Verificar Password
  │                      │
  │                      ├─ Generar JWT
  │                      │
  │    ◄─ JWT + Usuario─┤
  │       (200 OK)       │
  │                      │
  ├─ GET /games ─────────►│
  │  Header: Authorization: Bearer <JWT>
  │                      │
  │                      ├─ Validar JWT
  │                      │
  │                      ├─ Verificar Claims
  │                      │
  │                      ├─► Obtener Juegos──►│
  │                      │◄─── Juegos───────────
  │                      │
  │    ◄─ Juegos (JSON)─┤
  │       (200 OK)       │
  │                      │
```

---

## 🖥️ Entorno de Desarrollo

### Especificaciones del Sistema

| Item | Valor |
|------|-------|
| **IDE** | Visual Studio Community 2026 (18.6.1) |
| **Sistema Operativo** | Windows |
| **SDK .NET** | 10.0.300 |
| **Shell Preferido** | PowerShell.exe |
| **Ruta Workspace** | C:\Users\PC\source\repos\NuevoForo\ |

### Herramientas y Extensiones Recomendadas

```
Visual Studio Community 2026
├── ASP.NET and web development workload
├── .NET desktop development workload
├── Entity Framework Core Power Tools
├── NuGet Package Manager
├── Git integration
└── REST Client (para testing de APIs)
```

### Configuración Recomendada

**appsettings.json** (variables de entorno a configurar):
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Host=localhost;Database=NuevoForo;Username=postgres;Password=***"
  },
  "Jwt": {
	"Issuer": "NuevoForo",
	"Audience": "NuevoForo",
	"SigningKey": "DEBE_TENER_MAS_DE_32_CARACTERES",
	"ExpirationMinutes": 60
  },
  "RateLimiting": {
	"Global": {
	  "PermitLimit": 100,
	  "WindowSeconds": 60
	},
	"Auth": {
	  "PermitLimit": 10,
	  "WindowSeconds": 60
	}
  }
}
```

### Versión de Paquetes Críticos

```
.NET: 10.0.0 (Preview)
Entity Framework Core: 10.0.0-preview.5
ASP.NET Core: 10.0.0-preview.5
C#: 13.0 (con .NET 10)
```

---

## 🚀 Guía Rápida

### Preparación Inicial

```bash
# 1. Clonar o navegar al repositorio
cd C:\Users\PC\source\repos\NuevoForo\

# 2. Restaurar dependencias
dotnet restore

# 3. Crear la base de datos
dotnet ef database update -p src/NuevoForo.Infrastructure

# 4. Ejecutar la aplicación
dotnet run -p src/NuevoForo.Api

# La API estará disponible en: https://localhost:5001
# Swagger UI: https://localhost:5001/swagger
```

### Comandos Útiles

```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion -p src/NuevoForo.Infrastructure -s src/NuevoForo.Api

# Aplicar migraciones
dotnet ef database update -p src/NuevoForo.Infrastructure

# Revertir la última migración
dotnet ef database update NombreMigracionAnterior -p src/NuevoForo.Infrastructure

# Ejecutar tests
dotnet test

# Construir solución
dotnet build

# Compilar en release
dotnet build -c Release
```

### Estructura de Directorios

```
C:\Users\PC\source\repos\NuevoForo\
├── src/
│   ├── NuevoForo.Api/           ← Inicia aquí
│   ├── NuevoForo.Application/   ← DTOs e interfaces
│   ├── NuevoForo.Domain/        ← Entidades
│   └── NuevoForo.Infrastructure/ ← Servicios y BD
├── tests/
│   └── NuevoForo.Api.UnitTests/  ← Pruebas unitarias
├── data/
│   └── steam_games_sample.json  ← Datos de ejemplo
└── NuevoForo.slnx               ← Archivo solución
```

### Testing de Endpoints

**Opción 1: Swagger UI**
```
Abrir: https://localhost:5001/swagger
Probar endpoints directamente en la interfaz
```

**Opción 2: NuevoForo.Api.http**
```
Archivo REST Client en VS
POST https://localhost:5001/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Opción 3: cURL**
```bash
curl -X POST https://localhost:5001/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Password123!"}'
```

---

## 📝 Convenciones de Código

### Nombrado

- **Entidades:** PascalCase (ej: `Usuario`, `Juego`, `Resena`)
- **DTOs:** PascalCase + sufijo (ej: `LoginRequest`, `GameResponse`)
- **Métodos:** PascalCase (ej: `GetUserAsync`, `CreateGameAsync`)
- **Propiedades:** PascalCase (ej: `Id`, `Nombre`)
- **Variables privadas:** camelCase con _ (ej: `_dbContext`)

### Patrones

- Métodos async: sufijo `Async` (ej: `GetAllGamesAsync`)
- CancellationToken: parámetro opcional en métodos async
- Logging: ILogger inyectado en servicios
- Excepciones: throw con ArgumentNullException, etc.

---

## 📚 Referencias Útiles

- [Microsoft Docs - ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [JWT.io - JWT Debugger](https://jwt.io/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

---

**Documento compilado:** 27/05/2026  
**Ambiente:** Desarrollo  
**Estado:** ✅ Sistema Funcional

