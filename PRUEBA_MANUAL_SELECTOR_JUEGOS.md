# Guía de Prueba Manual - Selector de Juegos UGC

## 🚀 Requisitos
- Backend corriendo en `http://localhost:5000`
- Frontend corriendo en `http://localhost:5174` (o el puerto que indique)
- Usuario autenticado en la plataforma
- Mínimo 1 juego en la BD

## 📋 Pasos de Prueba

### 1. Navegar al formulario de UGC
- Ir a `http://localhost:5174/ugc/upload`
- Verificar que el componente carga sin errores en la consola

### 2. Verificar carga de juegos
✅ **Esperado:**
- El campo de búsqueda aparece
- Se muestra mensaje "⏳ Cargando juegos..." inicialmente
- Después de 1-2 segundos, desaparece el mensaje de carga
- El input está habilitado

### 3. Prueba de búsqueda
**Acción:** Escribe el nombre de un juego (ej: "Elden", "Dark", etc.)

✅ **Esperado:**
- El dropdown aparece mostrando juegos filtrados
- Se muestran máximo 10 juegos
- Cada juego muestra: thumbnail (si existe) + nombre
- Si no hay coincidencias, muestra "No se encontraron juegos"

### 4. Seleccionar un juego
**Acción:** Click en uno de los juegos del dropdown

✅ **Esperado:**
- El dropdown desaparece
- Se muestra sección "Juego seleccionado" con:
  - Thumbnail del juego (si existe)
  - Nombre del juego
  - Botón "Cambiar juego"
- El ID del juego se guarda internamente

### 5. Cambiar de juego
**Acción:** Click en botón "Cambiar juego"

✅ **Esperado:**
- Se limpia la selección
- Vuelve a mostrar el input de búsqueda vacío
- El dropdown se reinicia

### 6. Llenar formulario completo
**Acción:** Completa los campos:
- Título: "Prueba de UGC"
- Descripción: "Descripción de prueba"
- Juego: Selecciona cualquier juego
- Tags: "test, prueba"
- Archivo: Selecciona un archivo (.zip, .pdf, etc.)
- ✅ Checkbox: Acepta declaración legal

✅ **Esperado:**
- Todos los campos se llenan sin errores
- El thumbnail del juego es visible
- El nombre del archivo aparece debajo del input file

### 7. Publicar UGC
**Acción:** Click en botón "🚀 Publicar"

✅ **Esperado:**
- Botón cambia a "⏳ Publicando..."
- Se deshabilitan los inputs
- Después de 2-5 segundos, se muestra: "✅ UGC publicado correctamente."
- El formulario se limpia completamente
- Mensaje de éxito desaparece después de 4 segundos

### 8. Validación de errores

**Caso 1: Sin juego seleccionado**
- Intenta publicar sin seleccionar juego
- ✅ Esperado: Muestra "❌ Debes seleccionar un juego."

**Caso 2: Sin archivo**
- Selecciona juego, completa título, pero NO selecciona archivo
- ✅ Esperado: Muestra "❌ Completa título y selecciona un archivo."

**Caso 3: Sin aceptar declaración legal**
- Completa todo pero NO marca el checkbox
- ✅ Esperado: Muestra "❌ Debes aceptar la declaración legal."

## 🔍 Verificaciones técnicas

### Backend
1. Abre DevTools (F12) → Network
2. Busca una solicitud GET a `/games/select`
   - ✅ Status: 200
   - ✅ Response: Array de GameSelectDto con Id, Nombre, ImagenPortadaUrl

3. Busca una solicitud POST a `/ugc`
   - ✅ Status: 201 Created
   - ✅ Headers: Content-Type: multipart/form-data
   - ✅ Response: UgcResponse con ID del contenido creado

### Frontend (Console)
1. Abre DevTools (F12) → Console
2. Debe estar limpia (sin errores rojos)
3. Si hay logs, deben ser informativos solamente

### CSS Responsive
1. Prueba en viewport de distintos tamaños:
   - Desktop (1920x1080): Dropdown normal
   - Tablet (768x1024): Responsive funcional
   - Mobile (375x667): Layout ajustado, selector legible

## 🐛 Posibles errores y soluciones

### "Network error" al cargar juegos
**Solución:** 
- Verifica que backend está corriendo en puerto 5000
- Revisa que CORS está configurado en Program.cs
- Check consola backend para errores

### "No se pudieron cargar los juegos disponibles"
**Solución:**
- Verifica que hay juegos en la BD (tabla Juegos)
- Revisa que el endpoint GET /api/games/select funciona
- Test: `curl http://localhost:5000/games/select`

### El juego no se guarda en el archivo
**Solución:**
- Verifica que JuegoId se envía en FormData
- Revisa logs del backend en UgcController
- Verifica que el juego existe en BD

### Error 404 "El juego con ID X no existe"
**Solución:**
- Verifica que seleccionaste un juego válido
- Verifica que el juego existe en la BD
- Puede haber sincronización de datos, actualiza la página

## ✅ Criterios de Aceptación

- [x] Selector de juegos funciona con búsqueda en tiempo real
- [x] Dropdown muestra thumbnails e nombres
- [x] Se puede seleccionar y cambiar de juego
- [x] El juego seleccionado se envía al backend
- [x] Backend valida que el juego existe
- [x] UGC se crea exitosamente
- [x] Formulario se limpia después de publicar
- [x] Validaciones funcionan en frontend
- [x] No hay errores en consola
- [x] Responsive en mobile/tablet/desktop

## 📝 Notas

- El upload de archivos es de desarrollo (sin restricciones)
- En producción se agregará:
  - Restricción de tipos de archivo
  - Validación de virus
  - Compresión de thumbnails
  - CDN para almacenamiento
