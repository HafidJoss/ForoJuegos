# Arquitectura y Tecnologías del Sistema

**Versión:** 0.1  
**Fecha:** 2026-05-19  
**Alcance:** Plataforma web de comunidad gamer para reseñas, clasificación y contenido UGC legal.

---

## 1. Tecnologías seleccionadas

### 1.1 Backend
- **Lenguaje:** C#  
- **Framework:** ASP.NET Core Web API  
- **ORM:** Entity Framework Core  

### 1.2 Frontend
- **Opción A (Full .NET):** Blazor WebAssembly  
- **Opción B:** React + .NET API  

### 1.3 Base de datos
- **PostgreSQL** (recomendado)  
  - Alternativa: **SQL Server**  

### 1.4 Cache y rendimiento
- **Redis** (cache, sesiones, rate limiting)

### 1.5 Almacenamiento de archivos
- **Azure Blob Storage** / **Amazon S3** / **MinIO**  
- Validación de metadatos + política de contenido legal

### 1.6 Búsqueda
- **PostgreSQL Full Text Search** (MVP)  
- **Elasticsearch/OpenSearch** (escala futura)

### 1.7 Autenticación y seguridad
- **ASP.NET Identity**
- **OAuth 2.0 (Google Login)**
- **JWT** para API

### 1.8 Observabilidad
- **Serilog** (logs estructurados)
- **OpenTelemetry** (traces y métricas)
- **Prometheus + Grafana** (opcional)

---

## 2. Arquitectura del sistema

### 2.1 Estilo arquitectónico
- **Monolito modular (MVP)** con separación por capas:
  - Presentación (Frontend)
  - Aplicación (API)
  - Dominio (lógica de negocio)
  - Infraestructura (DB, cache, storage)

### 2.2 Diagrama lógico (alto nivel)
Usuario → Frontend (Web) → API (.NET) → DB/Cache/Storage

### 2.3 Módulos principales
- Identidad y usuarios
- Reseñas y comentarios
- Clasificación y etiquetas
- UGC (subida/gestión)
- Moderación y reportes
- Donaciones

---

## 3. Protocolos y estándares

### 3.1 Comunicación
- **HTTPS obligatorio**
- **REST API** con JSON
- **WebSockets/SignalR** (notificaciones en tiempo real – opcional)

### 3.2 Autenticación
- **OAuth 2.0** (Google)
- **JWT** para proteger endpoints

### 3.3 Seguridad
- CORS controlado
- Rate limiting por IP/usuario
- Validación de entradas (servidor)
- Escaneo antivirus en archivos (TBD)

---

## 4. Garantías de velocidad y funcionalidad

### 4.1 Rendimiento
- Cache Redis para:
  - Listados de categorías
  - Reseñas populares
  - Perfiles frecuentes
- Índices en DB:
  - Usuario, juego, género, tags
- Compresión HTTP (gzip/brotli)
- Paginación en listados

### 4.2 Escalabilidad
- Storage externo para archivos (no en el servidor)
- Separación de servicios futura:
  - Moderación
  - Notificaciones
  - Búsqueda

### 4.3 Disponibilidad
- Deployment en cloud con balanceo (Azure/AWS)
- Backup diario de base de datos
- Health checks y alertas

### 4.4 Calidad funcional
- Suite de pruebas:
  - Unitarias (xUnit)
  - Integración (TestContainers)
  - End-to-end (Playwright)
- CI/CD con GitHub Actions

---

## 5. Decisiones abiertas (TBD)
1. Frontend definitivo (Blazor vs React).
2. Proveedor de storage (Azure, AWS, MinIO).
3. Uso de SignalR para notificaciones.
4. CDN para archivos públicos.
5. Política de caché y TTL.
6. Estrategia de antivirus para archivos.

---

## 6. Recomendación inicial (MVP)
- ASP.NET Core + PostgreSQL + Redis  
- Blazor WASM  
- Azure Blob Storage  
- REST API + JWT  
- Sin microservicios en fase 1