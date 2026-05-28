# Backlog Priorizado – Comunidad Gamer (MVP)

**Versión:** 0.1  
**Fecha:** 2026-05-19  
**Objetivo:** Organizar los entregables del MVP por prioridad y dependencias.

---

## P0 – Crítico (MVP imprescindible)

### EPIC 1: Autenticación y Usuarios
- **US-01** Registro de usuario (email/username/password)
- **US-02** Inicio de sesión
- **US-03** Login con Google
- **US-04** Recuperación de contraseña
- **US-05** Ver perfil público
- **US-06** Editar perfil
- **US-07** Roles básicos (usuario/moderador/admin)

### EPIC 2: Juegos y Catálogo
- **US-08** Crear/gestionar catálogo de juegos (admin)
- **US-09** Navegar catálogo con filtros básicos
- **US-10** Buscar juegos por texto, género y tags
- **US-11** Ver detalle de juego

### EPIC 3: Reseñas y Comentarios
- **US-12** Crear reseña con rating
- **US-13** Editar reseña propia
- **US-14** Eliminar reseña propia
- **US-15** Comentar reseña
- **US-16** Eliminar comentario propio
- **US-17** Like por reseña (1 por usuario)

### EPIC 4: UGC Legal (Subida y Gestión)
- **US-18** Subir contenido UGC con aceptación legal obligatoria
- **US-19** Publicar UGC automáticamente
- **US-20** Editar UGC propio
- **US-21** Eliminar UGC propio
- **US-22** Descargar UGC

### EPIC 5: Reportes y Moderación
- **US-23** Reportar contenido o usuario
- **US-24** Cola de reportes para moderadores
- **US-25** Acciones de moderación (ocultar/eliminar/advertir/suspender)
- **US-26** Registro de acción y cierre de reporte

---

## P1 – Importante (post-MVP cercano)

### EPIC 6: Notificaciones
- **US-27** Notificaciones básicas (comentarios, likes, reportes resueltos)
- **US-28** Marcar notificaciones como leídas

### EPIC 7: Donaciones
- **US-29** Integración con proveedor de pagos
- **US-30** Confirmación de donación
- **US-31** Badge para donadores

### EPIC 8: Rendimiento y Seguridad
- **US-32** Cache Redis para catálogo y reseñas populares
- **US-33** Rate limiting por IP/usuario
- **US-34** Auditoría básica de acciones críticas

---

## P2 – Deseable (futuro)

### EPIC 9: Búsqueda avanzada
- **US-35** Ranking de resultados por relevancia
- **US-36** Autocompletado en búsqueda
- **US-37** Filtros avanzados combinados

### EPIC 10: Escalabilidad y UX
- **US-38** CDN para archivos públicos
- **US-39** SignalR para notificaciones en tiempo real
- **US-40** Separación de servicios (moderación/búsqueda)

### EPIC 11: Legal y cumplimiento avanzado
- **US-41** Escaneo antivirus automático
- **US-42** Hash y verificación de archivos
- **US-43** Política de retención y borrado seguro

---

## Dependencias clave
- **UGC Legal** depende de definición de tipos/tamaño/antivirus (TBD).
- **Donaciones** depende de proveedor de pagos elegido.
- **Notificaciones** depende de modelo de eventos (mínimo).
- **Búsqueda avanzada** depende de elección de motor (Postgres FTS vs Elastic).

---

## Pendientes críticos (TBD)
1. Tipos de archivo permitidos  
2. Tamaño máximo por archivo  
3. Estrategia de antivirus  
4. SLA de resolución de reportes  
5. Idiomas soportados  

---