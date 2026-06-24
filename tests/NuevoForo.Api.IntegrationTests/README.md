# 🧪 Pruebas de Integración - NuevoForo

Proyecto de pruebas de integración para **NuevoForo** usando **Testcontainers** con **PostgreSQL**.

## 📋 Tabla de Contenidos

- [Requisitos](#requisitos)
- [Inicio Rápido](#inicio-rápido)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Ejecutar Pruebas](#ejecutar-pruebas)
- [Escribir Nuevas Pruebas](#escribir-nuevas-pruebas)
- [Troubleshooting](#troubleshooting)

---

## 📦 Requisitos

### Obligatorios
- **.NET 10 SDK** - [Descargar](https://dotnet.microsoft.com/download)
- **Docker Desktop** - [Descargar](https://www.docker.com/products/docker-desktop)
- **PostgreSQL** (dentro de Docker, no necesitas instalación local)

### Verificar Requisitos

```powershell
# Verificar .NET
dotnet --version

# Verificar Docker
docker --version

# Asegúrate que Docker está ejecutándose
docker ps
```

---

## 🚀 Inicio Rápido

### 1. Clonar o Navegar al Repositorio

```powershell
cd C:\Users\PC\source\repos\NuevoForo\tests\NuevoForo.Api.IntegrationTests
```

### 2. Restaurar Paquetes NuGet

```powershell
dotnet restore
```

### 3. Ejecutar Todas las Pruebas

```powershell
dotnet test --logger:console
```

### 4. Ejecutar Pruebas por Categoría

```powershell
# Solo pruebas de Usuarios
dotnet test --filter "FullyQualifiedName~Users"

# Solo pruebas de Reseñas
dotnet test --filter "FullyQualifiedName~Reviews"

# Solo pruebas de Comentarios
dotnet test --filter "FullyQualifiedName~Comments"

# Solo pruebas de Likes
dotnet test --filter "FullyQualifiedName~Likes"

# Solo pruebas de Juegos
dotnet test --filter "FullyQualifiedName~Games"
```

### 5. Ejecutar una Prueba Específica

```powershell
dotnet test --filter "Name=CreateUser_WithValidData_ShouldPersistInDatabase"
```

---

## 📁 Estructura del Proyecto

```
tests/NuevoForo.Api.IntegrationTests/
│
├── Fixtures/
│   ├── TestContainerFixture.cs          # Maneja el contenedor PostgreSQL
│   └── DatabaseFixture.cs               # Helpers para seeding y limpieza
│
├── Helpers/
│   ├── TestDataBuilder.cs               # Builders fluidos para entidades
│   ├── ContainerManager.cs              # Utilidades para contenedores
│   └── AssertionExtensions.cs           # Extensiones de assertions
│
├── Integration/
│   ├── Users/
│   │   └── UsersIntegrationTests.cs     # 7 pruebas de Usuarios
│   ├── Games/
│   │   └── GamesIntegrationTests.cs     # 8 pruebas de Juegos
│   ├── Reviews/
│   │   └── ReviewsIntegrationTests.cs   # 9 pruebas de Reseñas
│   ├── Comments/
│   │   └── CommentsIntegrationTests.cs  # 10 pruebas de Comentarios
│   └── Likes/
│       └── LikesIntegrationTests.cs     # 10 pruebas de Likes
│
├── appsettings.json                     # Configuración de logging
├── NuevoForo.Api.IntegrationTests.csproj # Definición del proyecto
└── README.md                            # Este archivo
```

---

## 🧪 Ejecutar Pruebas

### Opción 1: Desde Visual Studio

1. Abre **Visual Studio Community 2026**
2. Abre **Test Explorer** (Menú → Test → Windows → Test Explorer)
3. Haz clic en **Run All Tests in View** (▶️)
4. O selecciona una prueba específica y haz clic en **Run Selected Tests**

### Opción 2: Desde PowerShell

```powershell
# Ejecutar todas las pruebas
dotnet test

# Ejecutar con más detalle
dotnet test --verbose

# Ejecutar con output en tiempo real
dotnet test --logger:console

# Ejecutar con patrón específico
dotnet test --filter "TestMethod" --verbosity normal

# Ejecutar una clase completa
dotnet test --filter "FullyQualifiedName~UsersIntegrationTests"
```

### Opción 3: Ejecutar con VSTest (si prefieres)

```powershell
dotnet vstest NuevoForo.Api.IntegrationTests.dll --logger:console
```

---

## 📝 Escribir Nuevas Pruebas

### Ejemplo: Agregar Prueba para Entidad Nueva

#### 1. Crear archivo de prueba

```powershell
# Crear nuevo archivo (ejemplo: Donations)
New-Item -Path "Integration/Donations/DonationsIntegrationTests.cs"
```

#### 2. Estructura básica de prueba

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuevoForo.Api.IntegrationTests.Fixtures;
using NuevoForo.Api.IntegrationTests.Helpers;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Api.IntegrationTests.Integration.Donations;

[TestClass]
public class DonationsIntegrationTests : IAsyncLifetime
{
	private DatabaseFixture _fixture = null!;
	private AppDbContext _dbContext = null!;

	public async Task InitializeAsync()
	{
		_fixture = new DatabaseFixture();
		await _fixture.InitializeAsync();
		_dbContext = _fixture.DbContext;
	}

	public async Task DisposeAsync()
	{
		await _fixture.DisposeAsync();
	}

	[TestMethod]
	[Description("Verifica que se puede crear una donación")]
	public async Task CreateDonation_WithValidData_ShouldPersistInDatabase()
	{
		// Arrange
		var donation = new Donacion
		{
			Id = Guid.NewGuid(),
			UsuarioId = (await _fixture.GetOrCreateTestUserAsync()).Id,
			Monto = 50.00m,
			FechaCreacion = DateTime.UtcNow,
			Activo = true
		};

		// Act
		_dbContext.Donaciones.Add(donation);
		await _dbContext.SaveChangesAsync();

		// Assert
		var saved = await _dbContext.Donaciones
			.FirstOrDefaultAsync(d => d.Id == donation.Id);

		Assert.IsNotNull(saved);
		Assert.AreEqual(50.00m, saved.Monto);
	}
}
```

#### 3. Usar TestDataBuilder (si hay builder)

```csharp
// Si existe builder en TestDataBuilder.cs
var donation = TestDataBuilder.CreateDonation()
	.WithMonto(100.00m)
	.Build();
```

#### 4. Patrón AAA (Arrange-Act-Assert)

```
// ARRANGE: Preparar datos
var user = await _fixture.GetOrCreateTestUserAsync();

// ACT: Ejecutar la acción
var result = await _dbContext.Users
	.FirstOrDefaultAsync(u => u.Id == user.Id);

// ASSERT: Verificar resultado
Assert.IsNotNull(result);
Assert.AreEqual(user.Id, result.Id);
```

### Naming Convention para Pruebas

```
[MethodName]_[Condition]_[ExpectedResult]

Ejemplos:
- CreateUser_WithValidData_ShouldPersistInDatabase
- UpdateReview_WithModifiedData_ShouldPersistChanges
- DeleteComment_ShouldMarkAsInactive
- SearchGames_ByNombre_ShouldReturnMatches
```

### Agregar a TestDataBuilder (si es nuevo tipo)

1. Abre `Helpers/TestDataBuilder.cs`
2. Agrega un builder interno:

```csharp
public class DonationBuilder
{
	private Guid _id = Guid.NewGuid();
	private decimal _monto = 50.00m;
	// ... más campos

	public DonationBuilder WithMonto(decimal monto)
	{
		_monto = monto;
		return this;
	}

	public Donacion Build()
	{
		return new Donacion { Id = _id, Monto = _monto, /* ... */ };
	}
}

// Al inicio de TestDataBuilder
public static DonationBuilder CreateDonation() => new();
```

---

## 🐛 Troubleshooting

### ❌ Error: "Docker daemon is not running"

**Solución:**
```powershell
# Inicia Docker Desktop
Start-Process -FilePath "C:\Program Files\Docker\Docker\Docker Desktop.exe"

# O desde WSL si usas WSL2:
wsl --list --verbose
```

### ❌ Error: "Connection refused" al conectar a PostgreSQL

**Solución:**
```powershell
# Verifica que Docker está corriendo
docker ps

# Verifica los logs del contenedor
docker logs <container_id>

# Limpia contenedores viejos
docker container prune
```

### ❌ Error: "MigrationsAssembly not found"

**Solución:**
```powershell
# Asegúrate que NuevoForo.Infrastructure está referenciado
dotnet restore

# Rebuild
dotnet build --no-restore
```

### ❌ Error: "Timeout waiting for container"

**Solución:**
- Aumenta el tiempo de espera en `ContainerManager.cs`
- Verifica recursos de Docker (CPU/Memory)
- Intenta con imagen más ligera de PostgreSQL

```csharp
// En TestContainerFixture.cs, aumenta delay:
// private const int StartupDelayMs = 5000; // más tiempo
```

### ❌ Error: "Port already in use"

**Solución:**
```powershell
# Testcontainers asigna puerto aleatorio automáticamente
# Si persiste:
docker ps -a
docker rm <container_id>
```

### ✅ Las pruebas se ejecutan lentamente

**Razones comunes:**
1. Primera ejecución (descarga imagen PostgreSQL)
2. Disco lento (SSD recomendado)
3. Docker asignado poco CPU/RAM

**Soluciones:**
- Aumenta recursos en Docker Desktop (Settings → Resources)
- Ejecuta pruebas en paralelo si es posible
- Usa disco SSD en lugar de HDD

---

## 📊 Estadísticas de Pruebas

| Módulo | Pruebas | Tiempo |
|--------|---------|--------|
| Users | 7 | ~5-7s |
| Games | 8 | ~5-7s |
| Reviews | 9 | ~5-7s |
| Comments | 10 | ~5-7s |
| Likes | 10 | ~5-7s |
| **TOTAL** | **44** | **~30-40s** |

---

## 🔄 Ciclo de Vida de Pruebas

```
[Test 1]
├─ InitializeAsync()
│  ├─ Crear TestContainerFixture
│  ├─ Iniciar contenedor PostgreSQL
│  ├─ Aplicar migraciones
│  └─ Crear DbContext
├─ [Ejecutar prueba]
└─ DisposeAsync()
   ├─ Cerrar DbContext
   ├─ Parar contenedor
   └─ Eliminar contenedor

[Test 2]
├─ InitializeAsync() [NUEVO contenedor]
├─ [Ejecutar prueba]
└─ DisposeAsync()

[...]
```

**Cada prueba obtiene un contenedor limpio y aislado.**

---

## 🎯 Mejores Prácticas

✅ **DO:**
- Usa `DatabaseFixture` para seeding
- Usa `TestDataBuilder` para entidades complejas
- Una asercción principal por prueba
- Nombres claros (patrón AAA)
- Limpia datos entre pruebas

❌ **DON'T:**
- No dependas de pruebas previas
- No modifiques `appsettings.json` en pruebas
- No uses datos hardcoded (usa builders)
- No ignores excepciones esperadas
- No dejes contenedores corriendo

---

## 🚀 Próximos Pasos

1. **Ejecuta todas las pruebas:** `dotnet test`
2. **Verifica que pasen:** 44 pruebas deberían pasar
3. **Agrega más pruebas** según necesites
4. **Integra en CI/CD** (GitHub Actions) en el futuro

---

## 📞 Soporte

Para problemas:
1. Verifica logs: `dotnet test --logger:console --verbosity diagnostic`
2. Consulta Docker logs: `docker logs <container>`
3. Limpia y reintenta: `docker system prune -a`

---

## 📄 Licencia

Este proyecto es parte de **NuevoForo** - [GitHub](https://github.com/HafidJoss/ForoJuegos)

---

**Última actualización:** Junio 2026  
**Versión:** 1.0.0
