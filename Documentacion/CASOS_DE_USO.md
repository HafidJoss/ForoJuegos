# Casos de Uso – Sistema Web Comunidad Gamer (MVP)

**Versión:** 1.0  
**Fecha:** 2026-05-19  
**Objetivo:** Definir la lista completa de casos de uso y sus especificaciones detalladas para construir la primera versión funcional del sistema.

---

## 1. Lista de casos de uso (MVP)

1. UC-01 Registro de usuario  
2. UC-02 Inicio de sesión  
3. UC-03 Inicio de sesión con Google  
4. UC-04 Recuperar contraseña  
5. UC-05 Ver perfil de usuario  
6. UC-06 Editar perfil  
7. UC-07 Crear reseña de videojuego  
8. UC-08 Editar reseña  
9. UC-09 Eliminar reseña  
10. UC-10 Comentar reseña  
11. UC-11 Eliminar comentario  
12. UC-12 Valorar reseña (like)  
13. UC-13 Navegar catálogo de juegos  
14. UC-14 Buscar juegos/etiquetas  
15. UC-15 Ver detalle de juego  
16. UC-16 Subir contenido UGC legal  
17. UC-17 Editar contenido UGC  
18. UC-18 Eliminar contenido UGC  
19. UC-19 Descargar contenido UGC  
20. UC-20 Reportar contenido/usuario  
21. UC-21 Moderar reportes  
22. UC-22 Donar  
23. UC-23 Ver notificaciones básicas  

---

## 2. Especificaciones detalladas de casos de uso

### UC-01 Registro de usuario
**Actor:** Visitante  
**Objetivo:** Crear cuenta en la plataforma.  
**Precondiciones:** No estar autenticado.  
**Flujo principal:**
1. El visitante abre el formulario de registro.
2. Ingresa correo, nombre de usuario y contraseña.
3. Acepta términos y política de contenido legal.
4. El sistema valida datos y crea la cuenta.
5. Se envía correo de verificación (opcional en MVP).
**Postcondiciones:** Usuario registrado.  
**Reglas:** Email único; contraseña segura.

---

### UC-02 Inicio de sesión
**Actor:** Usuario  
**Objetivo:** Autenticarse en el sistema.  
**Precondiciones:** Usuario registrado.  
**Flujo principal:**
1. Usuario ingresa email y contraseña.
2. Sistema valida credenciales.
3. Sistema crea sesión.
**Postcondiciones:** Usuario autenticado.  
**Reglas:** Bloqueo tras N intentos fallidos (TBD).

---

### UC-03 Inicio de sesión con Google
**Actor:** Usuario  
**Objetivo:** Autenticarse vía Google OAuth.  
**Precondiciones:** Cuenta Google activa.  
**Flujo principal:**
1. Usuario selecciona "Continuar con Google".
2. Se redirige a Google OAuth.
3. Usuario autoriza acceso.
4. Sistema crea o asocia cuenta.
**Postcondiciones:** Usuario autenticado.  

---

### UC-04 Recuperar contraseña
**Actor:** Usuario  
**Objetivo:** Restablecer contraseña.  
**Flujo principal:**
1. Usuario ingresa email.
2. Sistema envía enlace de restablecimiento.
3. Usuario define nueva contraseña.
**Postcondiciones:** Contraseña actualizada.

---

### UC-05 Ver perfil de usuario
**Actor:** Usuario / Visitante  
**Objetivo:** Ver perfil público.  
**Flujo principal:**  
1. El actor accede al perfil.  
2. El sistema muestra info pública (nombre, reseñas, contenido UGC).  

---

### UC-06 Editar perfil
**Actor:** Usuario  
**Objetivo:** Actualizar datos personales.  
**Flujo principal:**  
1. Usuario abre edición.  
2. Cambia nombre, bio, avatar, idioma.  
3. Guarda cambios.  

---

### UC-07 Crear reseña de videojuego
**Actor:** Usuario  
**Objetivo:** Publicar reseña con calificación.  
**Precondiciones:** Autenticado.  
**Flujo principal:**  
1. Usuario selecciona juego.  
2. Escribe reseña y rating (1–5).  
3. Publica.  
**Postcondiciones:** Reseña visible.

---

### UC-08 Editar reseña
**Actor:** Usuario (autor)  
**Objetivo:** Modificar reseña propia.  
**Flujo principal:**  
1. Usuario abre su reseña.  
2. Edita texto/rating.  
3. Guarda.  

---

### UC-09 Eliminar reseña
**Actor:** Usuario (autor) / Moderador  
**Objetivo:** Eliminar reseña.  
**Flujo principal:**  
1. Actor selecciona eliminar.  
2. Sistema solicita confirmación.  
3. Reseña pasa a estado “eliminada”.  

---

### UC-10 Comentar reseña
**Actor:** Usuario  
**Objetivo:** Responder reseña.  
**Flujo:**  
1. Usuario escribe comentario.  
2. Publica.  

---

### UC-11 Eliminar comentario
**Actor:** Usuario (autor) / Moderador  
**Objetivo:** Quitar comentario.  
**Flujo:** Confirmar → eliminar.

---

### UC-12 Valorar reseña (like)
**Actor:** Usuario  
**Objetivo:** Dar apoyo a reseña útil.  
**Flujo:**  
1. Usuario presiona “like”.  
2. Sistema registra la valoración.  
**Regla:** Un like por usuario por reseña.

---

### UC-13 Navegar catálogo de juegos
**Actor:** Usuario / Visitante  
**Objetivo:** Ver listado de juegos.  
**Flujo:**  
1. Actor abre catálogo.  
2. Sistema muestra juegos con filtros.  

---

### UC-14 Buscar juegos/etiquetas
**Actor:** Usuario / Visitante  
**Objetivo:** Encontrar juegos por texto, género, tags.  
**Flujo:**  
1. Actor ingresa criterio.  
2. Sistema devuelve resultados.  

---

### UC-15 Ver detalle de juego
**Actor:** Usuario / Visitante  
**Objetivo:** Ver información del juego.  
**Flujo:**  
1. Actor selecciona juego.  
2. Sistema muestra reseñas, rating promedio, UGC asociado.  

---

### UC-16 Subir contenido UGC legal
**Actor:** Usuario  
**Objetivo:** Publicar archivo legal (mods, parches, guías autorizadas).  
**Precondiciones:** Autenticado.  
**Flujo principal:**  
1. Usuario selecciona “Subir contenido”.  
2. Adjunta archivo y metadatos (título, descripción, juego, tags).  
3. Acepta declaración legal: “Tengo permiso para distribuir este contenido”.  
4. Sistema publica automáticamente.  
**Postcondiciones:** Archivo visible.  
**Reglas legales:**  
- Prohibido contenido con copyright sin autorización.  
- El usuario es responsable legal del contenido.  
- Contenido reportado puede ser removido.  

---

### UC-17 Editar contenido UGC
**Actor:** Usuario (autor)  
**Objetivo:** Modificar metadatos o reemplazar archivo.  
**Flujo:**  
1. Usuario abre contenido.  
2. Edita información o reemplaza archivo.  
3. Guarda.  

---

### UC-18 Eliminar contenido UGC
**Actor:** Usuario (autor) / Moderador  
**Objetivo:** Retirar contenido.  
**Flujo:** Confirmar → eliminar (o despublicar).

---

### UC-19 Descargar contenido UGC
**Actor:** Usuario / Visitante  
**Objetivo:** Descargar contenido legal.  
**Flujo:**  
1. Actor selecciona “descargar”.  
2. Sistema entrega archivo.  

---

### UC-20 Reportar contenido/usuario
**Actor:** Usuario  
**Objetivo:** Denunciar abuso o contenido ilegal.  
**Flujo:**  
1. Usuario presiona “Reportar”.  
2. Selecciona motivo.  
3. Envía.  
**Postcondición:** Reporte en cola de moderación.

---

### UC-21 Moderar reportes
**Actor:** Moderador/Administrador  
**Objetivo:** Resolver reportes.  
**Flujo principal:**  
1. Moderador revisa lista de reportes.  
2. Evalúa evidencia.  
3. Toma acción (ocultar, eliminar, advertir, suspender).  
4. Cierra reporte.  

---

### UC-22 Donar
**Actor:** Usuario  
**Objetivo:** Donar al proyecto.  
**Flujo:**  
1. Usuario accede a donar.  
2. Completa pago (proveedor externo).  
3. Sistema confirma y agrega badge.  

---

### UC-23 Ver notificaciones básicas
**Actor:** Usuario  
**Objetivo:** Ver interacciones relevantes.  
**Flujo:**  
1. Usuario abre notificaciones.  
2. Sistema muestra nuevos comentarios, likes, reportes resueltos.  

---

## 3. Reglas generales del sistema (MVP)
- Todo contenido UGC debe ser **legal** y con permiso del autor.
- El sistema permite reportar y retirar contenido infractor.
- Publicación de UGC es automática, pero con **términos obligatorios**.
- Los usuarios pueden eliminar su propio contenido.

---

## 4. Pendientes (TBD)
1. Tipos de archivo permitidos.  
2. Tamaño máximo de archivo.  
3. SLA de resolución de reportes.  
4. Configuración de notificaciones (email/push).  