# Requisitos del Proyecto – Foro de Comunidad Gamer

**Versión:** 0.1  
**Fecha:** 2026-05-19  
**Alcance:** Plataforma web para reseñas, comunidad y distribución de contenido UGC legal (mods/parches/recursos autorizados), con autenticación, moderación y donaciones.

---

## 1. Objetivo del sistema
Crear una comunidad web donde gamers puedan:
- Publicar reseñas y opiniones sobre videojuegos.
- Clasificar títulos por género y etiquetas.
- Compartir contenido **legal** creado por la comunidad (mods, parches, traducciones, guías, assets autorizados).
- Construir reputación y participación mediante interacción social.

---

## 2. Supuestos y restricciones legales (obligatorias)
Para garantizar el cumplimiento legal, el sistema **debe**:
- Prohibir contenido con copyright sin autorización.
- Exigir aceptación de términos antes de publicar contenido.
- Permitir denuncias y retiro rápido de contenido infractor.
- Permitir que el autor elimine su propio contenido.

> **Nota:** El sistema no apoyará mecanismos para distribuir videojuegos comerciales u otros archivos protegidos.

---

## 3. Requisitos funcionales (RF)

### 3.1 Usuarios y autenticación
- **RF-01:** El sistema permitirá registro e inicio de sesión.
- **RF-02:** El sistema permitirá inicio de sesión con Google.
- **RF-03:** El sistema tendrá roles: usuario, moderador, administrador.
- **RF-04:** Los usuarios podrán editar su perfil.

### 3.2 Reseñas y opiniones
- **RF-05:** Los usuarios podrán crear reseñas de videojuegos.
- **RF-06:** Las reseñas tendrán puntuación simple (1–5).
- **RF-07:** Los usuarios podrán comentar reseñas.
- **RF-08:** El autor podrá editar o eliminar sus reseñas.

### 3.3 Clasificación y búsqueda
- **RF-09:** El sistema permitirá clasificar juegos por género.
- **RF-10:** El sistema permitirá etiquetas libres.
- **RF-11:** El sistema permitirá búsqueda por texto, género y etiquetas.

### 3.4 Contenido UGC (archivos)
- **RF-12:** Los usuarios podrán subir contenido UGC **legal**.
- **RF-13:** Al subir contenido, el usuario deberá aceptar términos de distribución legal.
- **RF-14:** El contenido se publicará automáticamente.
- **RF-15:** El autor podrá editar o eliminar su contenido subido.

> **TBD:**  
> - Tipos de archivo permitidos.  
> - Tamaño máximo por archivo.  
> - Escaneo automático (antivirus/hash).

### 3.5 Reportes y moderación
- **RF-16:** Los usuarios podrán reportar contenido o usuarios.
- **RF-17:** Los moderadores tendrán una cola de reportes.
- **RF-18:** Los moderadores podrán ocultar o eliminar contenido.
- **RF-19:** Los administradores podrán suspender o banear usuarios.

### 3.6 Monetización
- **RF-20:** El sistema permitirá donaciones voluntarias.
- **RF-21:** Los usuarios que donen podrán tener un badge.

### 3.7 Multilenguaje
- **RF-22:** El sistema será multilenguaje (interfaz y contenido).

---

## 4. Requisitos no funcionales (RNF)

### 4.1 Seguridad
- **RNF-01:** Autenticación segura con OAuth (Google).
- **RNF-02:** Hash seguro de contraseñas (ASP.NET Identity).
- **RNF-03:** Protección contra XSS, CSRF, SQL Injection.
- **RNF-04:** Registro de actividad crítica (auditoría básica).

### 4.2 Legal y cumplimiento
- **RNF-05:** Términos y política de contenido obligatorios.
- **RNF-06:** Retiro rápido de contenido reportado (SLA TBD).
- **RNF-07:** Registro de aceptación de términos por usuario.

### 4.3 Rendimiento
- **RNF-08:** Respuesta promedio < 500ms en operaciones comunes.
- **RNF-09:** Cache para contenido consultado frecuentemente.

### 4.4 Escalabilidad
- **RNF-10:** Arquitectura preparada para crecimiento gradual.
- **RNF-11:** Posibilidad de escalar almacenamiento de archivos.

### 4.5 Disponibilidad
- **RNF-12:** Disponibilidad objetivo ≥ 99.5%.

### 4.6 Usabilidad
- **RNF-13:** Interfaz responsive (móvil y desktop).
- **RNF-14:** Diseño accesible (mínimo WCAG AA).

---

## 5. Requisitos tecnológicos (stack)
- **Backend:** ASP.NET Core Web API  
- **Frontend:** Blazor WebAssembly o React  
- **DB:** PostgreSQL o SQL Server  
- **Cache:** Redis  
- **Storage:** Azure Blob / S3 / MinIO  

---

## 6. Pendientes por definir (TBD)
1. Tamaño máximo por archivo.
2. Tipos de archivo permitidos.
3. Política de retención de contenido.
4. Escaneo antivirus automático.
5. Idiomas soportados (lista).
6. SLA para resolución de reportes.

---

## 7. Criterios de aceptación (MVP)
- Registro/login con Google.
- Publicación de reseñas con rating.
- Subida de contenido UGC legal.
- Reporte y moderación básica.
- Clasificación por género y búsqueda.