# Modelo de Datos Inicial (MVP) – Comunidad Gamer

**Versión:** 0.1  
**Fecha:** 2026-05-19  
**Objetivo:** Definir entidades base y relaciones para el MVP.

---

## 1. Entidades principales

### 1.1 Usuario
- Id (GUID)
- Email (único)
- Username (único)
- PasswordHash
- Nombre
- Bio
- AvatarUrl
- Idioma
- Rol (Usuario | Moderador | Admin)
- FechaRegistro
- Estado (Activo | Suspendido | Eliminado)
- UltimoLogin

### 1.2 Juego
- Id (GUID)
- Nombre
- Descripción
- GéneroPrincipal
- Tags (lista)
- FechaLanzamiento
- Plataforma
- ImagenPortadaUrl

### 1.3 Reseña
- Id (GUID)
- UsuarioId (FK)
- JuegoId (FK)
- Texto
- Rating (1–5)
- FechaCreación
- FechaActualización
- Estado (Activa | Eliminada)

### 1.4 Comentario
- Id (GUID)
- ReseñaId (FK)
- UsuarioId (FK)
- Texto
- FechaCreación
- Estado (Activo | Eliminado)

### 1.5 LikeReseña
- Id (GUID)
- ReseñaId (FK)
- UsuarioId (FK)
- FechaCreación
- **Regla:** único por (UsuarioId, ReseñaId)

### 1.6 ContenidoUGC
- Id (GUID)
- UsuarioId (FK)
- JuegoId (FK)
- Título
- Descripción
- ArchivoUrl
- ArchivoNombre
- ArchivoSize
- ArchivoHash (opcional)
- Tags (lista)
- DeclaraciónLegalAceptada (bool)
- FechaSubida
- FechaActualización
- Estado (Publicado | Oculto | Eliminado)

### 1.7 Reporte
- Id (GUID)
- ReportadoPorUsuarioId (FK)
- TipoObjetivo (Usuario | Reseña | Comentario | ContenidoUGC)
- ObjetivoId (GUID)
- Motivo
- Evidencia (texto/url)
- Estado (Abierto | En revisión | Resuelto | Rechazado)
- FechaCreación
- FechaCierre
- ModeradorId (FK, nullable)
- AcciónTomada (Ocultar | Eliminar | Advertir | Suspender | Ninguna)

### 1.8 Donación
- Id (GUID)
- UsuarioId (FK)
- Monto
- Moneda
- ProveedorPago
- TransacciónId
- Estado (Pendiente | Confirmada | Fallida)
- FechaCreación

### 1.9 Notificación
- Id (GUID)
- UsuarioId (FK)
- Tipo (Comentario | Like | ReporteResuelto | Sistema)
- Mensaje
- Leída (bool)
- FechaCreación

---

## 2. Relaciones clave
- Usuario 1..* Reseñas
- Usuario 1..* Comentarios
- Usuario 1..* ContenidoUGC
- Usuario 1..* Donaciones
- Usuario 1..* Notificaciones
- Juego 1..* Reseñas
- Juego 1..* ContenidoUGC
- Reseña 1..* Comentarios
- Reseña 1..* Likes
- Reporte apunta a Usuario | Reseña | Comentario | ContenidoUGC (polimórfico)

---

## 3. Índices y restricciones sugeridas
- Usuario.Email UNIQUE
- Usuario.Username UNIQUE
- LikeReseña UNIQUE (UsuarioId, ReseñaId)
- Reseña UNIQUE (UsuarioId, JuegoId) *(opcional, si se permite una reseña por juego)*
- Índices por JuegoId, UsuarioId en Reseñas, Comentarios, UGC
- Índices de texto en Juego.Nombre y Tags (para búsqueda)

---

## 4. Pendientes (TBD)
- Tipos y tamaños de archivo permitidos
- Estrategia de tags (tabla relacional vs array)
- Política de retención de contenido eliminado
- Auditoría y trazabilidad detallada

---