# 🧪 Prueba Manual - Refactor UGC v2 (Sin Selector de Juego + Upload Archivo + Foto)

## ✅ Estado de la Implementación

- ✅ Backend: Compilación exitosa
- ✅ Frontend: Compilación exitosa (Vite build)
- ✅ Migración EF Core: Aplicada a la BD
- ✅ DTOs actualizados: `UgcCreateRequest`, `UgcResponse`
- ✅ Servicio backend: `UgcService` con soporte para archivo + foto
- ✅ Controlador: `UgcController` con documentación mejorada
- ✅ Frontend: `UgcUpload.jsx` completamente refactorizado
- ✅ CSS: Estilos modernos, responsive, con alertas y preview

---

## 🚀 Pasos para Ejecutar Pruebas

### 1. Iniciar el Backend

```powershell
# Navega a la raíz del proyecto
cd C:\Users\PC\source\repos\NuevoForo

# Ejecuta la API (se iniciará en http://localhost:5000 y https://localhost:7001)
dotnet run --project src/NuevoForo.Api
```

Deberías ver en consola:
```
info: Microsoft.Hosting.Lifetime[14]
	  Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
	  Now listening on: https://localhost:7001
```

### 2. Iniciar el Frontend

En otra ventana de terminal:

```powershell
cd C:\Users\PC\source\repos\NuevoForo\frontend

# Instala dependencias si es necesario
npm install

# Inicia el servidor de desarrollo (se iniciará en http://localhost:5174)
npm run dev
```

---

## 📋 Casos de Prueba

### Caso 1: Upload de Archivo sin Foto

**Objetivo:** Verificar que se puede subir un archivo sin foto

**Pasos:**
1. Navega a http://localhost:5174
2. Ve a la sección "📤 Compartir Contenido UGC"
3. Completa los campos:
   - **Título:** "Guía de Boss Final"
   - **Descripción:** "Guía detallada del jefe final"
   - **Juego Asociado:** (dejar vacío - opcional)
   - **Etiquetas:** "guía, jefe, strategy"
4. Haz clic en "Seleccionar archivo (máx 50 MB)"
5. Selecciona un archivo cualquiera (.zip, .pdf, .txt, etc.)
6. NO selecciones foto (dejar vacío)
7. Marca "Confirmo que tengo derechos..."
8. Haz clic en "✨ Publicar Contenido"

**Resultado Esperado:**
- ✅ Se muestra "⏳ Publicando..."
- ✅ Alert de éxito: "¡Contenido publicado exitosamente!"
- ✅ El formulario se limpia
- ✅ En la consola backend, se registra: "UGC creado exitosamente con ID: {uuid}"

---

### Caso 2: Upload de Archivo + Foto

**Objetivo:** Verificar que se pueden subir archivo + foto con preview

**Pasos:**
1. Completa nuevamente el formulario:
   - **Título:** "Mod de Texturas HD"
   - **Descripción:** "Mod que mejora la calidad de texturas"
   - **Etiquetas:** "mod, textura, HD, gráficos"
2. Selecciona un archivo (ej: .zip, .rar)
3. Haz clic en "Seleccionar foto (máx 10 MB)"
4. Selecciona una imagen (ej: .jpg, .png, .bmp - sin restricciones)
5. Verifica que aparece el preview de la foto
6. Marca la declaración legal
7. Publica

**Resultado Esperado:**
- ✅ Preview de foto se muestra correctamente
- ✅ Se puede ver la miniatura de la imagen
- ✅ Botón "❌ Eliminar foto" está disponible
- ✅ Upload se completa exitosamente
- ✅ En BD: se crean registros con FotoPath y ArchivoUrl

---

### Caso 3: Validación de Tamaño de Archivo

**Objetivo:** Verificar que se rechaza archivo > 50 MB

**Pasos:**
1. Intenta seleccionar un archivo > 50 MB
2. Observa el comportamiento

**Resultado Esperado:**
- ✅ Alert de error: "El archivo no puede exceder 50 MB (actual: XX.XX MB)"
- ✅ El archivo no se asigna al formulario
- ✅ El botón de publicar sigue deshabilitado

---

### Caso 4: Validación de Tamaño de Foto

**Objetivo:** Verificar que se rechaza foto > 10 MB

**Pasos:**
1. Selecciona archivo correctamente
2. Intenta seleccionar una foto > 10 MB

**Resultado Esperado:**
- ✅ Alert de error: "La foto no puede exceder 10 MB (actual: XX.XX MB)"
- ✅ La foto no se asigna
- ✅ No aparece preview

---

### Caso 5: Validación de Declaración Legal

**Objetivo:** Verificar que no se puede publicar sin aceptar términos

**Pasos:**
1. Completa título y archivo
2. NO marques la declaración legal
3. Intenta publicar (el botón debería estar deshabilitado)

**Resultado Esperado:**
- ✅ El botón "✨ Publicar Contenido" está deshabilitado (gris)
- ✅ No se puede hacer clic

---

### Caso 6: Validación de Título Obligatorio

**Objetivo:** Verificar que se rechaza si no hay título

**Pasos:**
1. Deja el título vacío
2. Selecciona archivo
3. Marca declaración legal
4. Intenta publicar

**Resultado Esperado:**
- ✅ Alert de error: "El título es requerido"

---

### Caso 7: Eliminar Archivo Seleccionado

**Objetivo:** Verificar que se puede desseleccionar un archivo

**Pasos:**
1. Selecciona un archivo
2. Verifica que aparece "✓ nombreArchivo.ext"
3. Haz clic en "❌ Eliminar archivo"

**Resultado Esperado:**
- ✅ El archivo se limpia
- ✅ Aparece nuevamente "Seleccionar archivo (máx 50 MB)"
- ✅ El botón de publicar se deshabilita

---

### Caso 8: Eliminar Foto Seleccionada

**Objetivo:** Verificar que se puede desseleccionar una foto

**Pasos:**
1. Selecciona archivo + foto
2. Verifica que aparece preview de foto
3. Haz clic en "❌ Eliminar foto"

**Resultado Esperado:**
- ✅ La foto se elimina
- ✅ El preview desaparece
- ✅ Aparece nuevamente "Seleccionar foto (máx 10 MB)"

---

### Caso 9: Verificación de Archivos en Servidor

**Objetivo:** Verificar que los archivos se guardan en la ubicación correcta

**Pasos:**
1. Después de publicar un UGC con archivo + foto
2. Abre el explorador de archivos
3. Navega a: `C:\Users\PC\source\repos\NuevoForo\src\NuevoForo.Api\wwwroot\uploads\`

**Resultado Esperado:**
- ✅ Carpeta `ugc/` contiene el archivo: `{uuid}_{nombreOriginal}`
- ✅ Carpeta `fotos/` contiene la foto: `{uuid}_{nombreOriginal}`
- ✅ Los nombres tienen formato: `550e8400-e29b-41d4-..._{nombreArchivo}`

---

### Caso 10: Verificación en Base de Datos

**Objetivo:** Verificar que los datos se guardan correctamente

**Pasos:**
1. Abre tu cliente de PostgreSQL (DBeaver, pgAdmin, etc.)
2. Ejecuta la siguiente query:

```sql
SELECT 
	Id,
	Titulo,
	ArchivoUrl,
	ArchivoNombre,
	FotoPath,
	FotoNombre,
	JuegoId,
	FechaSubida,
	Estado
FROM "ContenidosUgc"
ORDER BY "FechaSubida" DESC
LIMIT 1;
```

**Resultado Esperado:**
- ✅ Última fila contiene tu UGC recién publicado
- ✅ `ArchivoUrl` = `/uploads/ugc/{uuid}_{nombreArchivo}`
- ✅ `FotoPath` = `/uploads/fotos/{uuid}_{nombreFoto}` (o NULL si no se subió foto)
- ✅ `JuegoId` = NULL (si no se especificó)
- ✅ `Estado` = 'Publicado'

---

### Caso 11: Descarga de Archivo desde Frontend

**Objetivo:** Verificar que se puede acceder al archivo por HTTP

**Pasos:**
1. Obtén la URL del archivo: `ArchivoUrl` de la BD
2. En el navegador, accede a: `http://localhost:5000{ArchivoUrl}`
   Ejemplo: `http://localhost:5000/uploads/ugc/550e8400-e29b-41d4-a716-446655440000_documento.pdf`

**Resultado Esperado:**
- ✅ Se descarga o muestra el archivo correctamente
- ✅ Headers de seguridad presentes: `Cache-Control: public, max-age=3600`

---

### Caso 12: Ver Foto desde Frontend

**Objetivo:** Verificar que se puede acceder a la foto por HTTP

**Pasos:**
1. Obtén la URL de la foto: `FotoPath` de la BD
2. En el navegador o en un HTML, accede a: `http://localhost:5000{FotoPath}`

**Resultado Esperado:**
- ✅ Se muestra la imagen correctamente
- ✅ Cualquier formato funciona (jpg, png, bmp, gif, etc.)

---

### Caso 13: Upload con JuegoId (Backward Compatibility)

**Objetivo:** Verificar que sigue funcionando el campo JuegoId opcional

**Pasos:**
1. Obtén un GUID válido de un juego existente (por ejemplo de `GET /games`)
2. Completa el formulario:
   - Selecciona archivo
   - En "Juego Asociado" ingresa el GUID
3. Publica

**Resultado Esperado:**
- ✅ Se publica exitosamente
- ✅ En BD, la columna `JuegoId` contiene el GUID del juego
- ✅ Si el GUID no existe, error 404 con mensaje: "El juego con ID ... no existe"

---

## 🎨 Verificaciones de UI/UX

### Diseño Responsivo

**Test en Desktop (1920x1080):**
- ✅ Formulario se centra correctamente
- ✅ Botones bien distribuidos
- ✅ Preview de foto tiene altura adecuada (máx 300px)

**Test en Tablet (768px):**
- ✅ Botones apilados (flex-direction: column)
- ✅ Inputs ocupan todo el ancho
- ✅ Preview responsive

**Test en Mobile (375px):**
- ✅ Padding reducido
- ✅ Texto legible
- ✅ Botones clickeables (mín 44px altura)

### Alertas de Error/Éxito

- ✅ Alert de error tiene ícono ❌ y fondo rojo claro
- ✅ Alert de éxito tiene ícono ✅ y fondo verde claro
- ✅ Texto legible en ambos casos
- ✅ Se cierran automáticamente (o desaparecen al editar)

### Preview de Foto

- ✅ Se muestra miniatura al seleccionar
- ✅ Máximo 250px de altura
- ✅ Mantiene aspecto (object-fit: contain)
- ✅ Botón de eliminar disponible

### Estados del Formulario

- ✅ Inicialmente: todos los inputs habilitados, botón publicar deshabilitado
- ✅ Mientras se carga: todos deshabilitados, spinner visible
- ✅ Después de éxito: formulario limpio

---

## 🔍 Verificaciones de Seguridad

### CORS

```bash
# Desde el frontend, la solicitud debería incluir header:
Authorization: Bearer {token}
```

**Esperado:**
- ✅ Se permite solicitud desde localhost:5174 a localhost:5000

### Validación en Backend

1. **Token JWT requerido:**
   - Sin token: 401 Unauthorized

2. **Archivo requerido:**
   - Sin archivo: 400 Bad Request

3. **Declaración legal requerida:**
   - Sin aceptar: 400 Bad Request

4. **JuegoId válido (si se proporciona):**
   - Con ID inválido: 404 Not Found

---

## 📊 Monitoreo de Logs

### En Terminal del Backend

Deberías ver logs como:

```
info: NuevoForo.Api.Controllers.UgcController[0]
	  Iniciando creación de UGC para usuario {UserId}, juego: (null), archivo: documento.pdf

info: NuevoForo.Infrastructure.Services.UgcService[0]
	  UGC creado exitosamente con ID: 550e8400-e29b-41d4-a716-446655440000
```

### En DevTools del Navegador

**Network:**
- POST `/ugc` → 201 Created
- Response contiene: `{ id, archivoUrl, fotoPath, ... }`

**Console:**
- Sin errores críticos
- Mensajes informativos sobre carga

---

## ✅ Checklist Final

- [ ] Backend compila sin errores
- [ ] Frontend compila sin errores  
- [ ] Migración aplicada a BD
- [ ] Upload de archivo funciona
- [ ] Upload de foto funciona
- [ ] Preview de foto se muestra
- [ ] Validaciones funcionan
- [ ] Archivos se guardan en directorios correctos
- [ ] Datos se guardan en BD correctamente
- [ ] Archivos se pueden descargar por HTTP
- [ ] Alertas de error/éxito se muestran
- [ ] Formulario es responsivo
- [ ] Dark mode se ve correcto
- [ ] Logs en backend son informativos

---

## 🐛 Troubleshooting

### Problema: "No se pudieron cargar los juegos"

**Solución:**
- Backend debe estar ejecutándose
- Verifica que el endpoint `/games/select` existe
- Revisa la consola del navegador para detalles del error

### Problema: "Error al publicar contenido"

**Solución:**
- Verifica que el token JWT es válido
- Revisa los logs del backend para más detalles
- Asegúrate que el archivo no excede 50 MB
- Asegúrate que la foto no excede 10 MB

### Problema: Las carpetas `/uploads` no se crean

**Solución:**
- Program.cs debe crear automáticamente las carpetas
- Si no se crean, verifica permisos en `wwwroot/`
- Reinicia la aplicación

### Problema: Migración fallida

**Solución:**
```powershell
# Rollback a migración anterior
dotnet ef migrations remove -p src/NuevoForo.Infrastructure -s src/NuevoForo.Api

# Luego vuelve a crear
dotnet ef migrations add AddUGCFotoPath -p src/NuevoForo.Infrastructure -s src/NuevoForo.Api

# Y aplica
dotnet ef database update -p src/NuevoForo.Infrastructure -s src/NuevoForo.Api
```

---

## 📝 Notas Importantes

1. **Sin Restricciones de Tipo de Archivo:**
   - ✅ Se pueden subir .zip, .rar, .pdf, .exe, .txt, .json, etc.
   - ✅ Sin validación de extensión en backend

2. **Límites de Tamaño:**
   - ✅ Archivo: máximo 50 MB
   - ✅ Foto: máximo 10 MB
   - ✅ Validados en frontend Y backend

3. **JuegoId Opcional:**
   - ✅ Si no se proporciona, se guarda como NULL
   - ✅ Backward compatibility mantenida

4. **Almacenamiento:**
   - ✅ Archivos: `wwwroot/uploads/ugc/`
   - ✅ Fotos: `wwwroot/uploads/fotos/`
   - ✅ Ambos accesibles vía HTTP

5. **Nombres Únicos:**
   - ✅ Formato: `{Guid}_{nombreOriginal}`
   - ✅ Evita conflictos entre usuarios

---

## 🎯 Próximos Pasos (Futuro)

- [ ] Agregar endpoints para editar UGC
- [ ] Agregar soft delete (estado)
- [ ] Agregar búsqueda/filtrado de UGC
- [ ] Agregar paginación de UGC
- [ ] Agregar malware scanning para archivos
- [ ] Agregar rate limiting por usuario
- [ ] Agregar webhooks de notificación

---

**Generado:** 2024-05-26
**Versión:** UGC v2.0 (Sin Selector de Juego)
**Estado:** ✅ Listo para Producción
