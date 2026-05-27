# 🚀 Guía Paso a Paso: Desplegar en Railway

## Paso 1: Crear Cuenta en Railway

1. Ve a **https://railway.app**
2. Haz clic en **Start Free**
3. Crea una cuenta usando:
   - GitHub (recomendado)
   - Email
   - Google
4. Verifica tu email si es necesario

---

## Paso 2: Crear un Nuevo Proyecto

1. Una vez en el dashboard, haz clic en **+ New Project**
2. Selecciona **Deploy from GitHub** (si conectaste GitHub)
   - O selecciona **Empty Project** si quieres usar ZIP
3. Conecta tu repositorio de GitHub:
   - Busca **NuevoForo**
   - Haz clic en **Deploy**

---

## Paso 3: Agregar Base de Datos PostgreSQL

**IMPORTANTE:** Haz esto ANTES de desplegar la API.

### Opción A: A través de la UI de Railway

1. En tu proyecto, haz clic en **+ New** o **Add Service**
2. Selecciona **Database**
3. Elige **PostgreSQL**
4. Railway creará automáticamente una instancia

### Opción B: CLI de Railway

```bash
# Instalar Railway CLI
npm install -g @railway/cli

# Inicia sesión
railway login

# En la carpeta del proyecto
cd C:\Users\PC\source\repos\NuevoForo\

# Crea un nuevo proyecto
railway init

# Agrega PostgreSQL
railway add --plugin postgres
```

---

## Paso 4: Obtener Cadena de Conexión

Railway generará automáticamente una variable de entorno:

1. En tu proyecto, haz clic en el servicio **PostgreSQL**
2. Ve a la pestaña **Variables**
3. Deberías ver: **DATABASE_URL** - Copia esta URL
4. También verás variables individuales:
   - `PGHOST`
   - `PGPORT` (5432 normalmente)
   - `PGUSER`
   - `PGPASSWORD`
   - `PGDATABASE`

**Ejemplo de DATABASE_URL:**
```
postgresql://usuario:contraseña@host.railway.app:5432/railway
```

---

## Paso 5: Configurar Variables de Entorno de la API

### En el Dashboard de Railway:

1. Haz clic en tu proyecto
2. Selecciona el servicio de la **API** (NuevoForo)
3. Ve a la pestaña **Variables**
4. Agrega las siguientes variables:

```
# Base de Datos
# La URL completa, Railway la proporciona automáticamente
# O construir manualmente desde variables de PostgreSQL
ConnectionStrings__DefaultConnection=postgresql://usuario:password@host:5432/railway

# JWT Configuration
Jwt__Issuer=NuevoForo
Jwt__Audience=NuevoForo
Jwt__SigningKey=GENERA_UNA_CLAVE_SEGURA_DE_AL_MENOS_32_CARACTERES
Jwt__ExpirationMinutes=60

# Rate Limiting
RateLimiting__Global__PermitLimit=100
RateLimiting__Global__WindowSeconds=60
RateLimiting__Auth__PermitLimit=10
RateLimiting__Auth__WindowSeconds=60

# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# Logging
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
```

**⚠️ IMPORTANTE:** La variable `Jwt__SigningKey` debe ser una cadena larga y segura:

```bash
# Generar una clave segura en PowerShell:
[System.Convert]::ToBase64String([System.Security.Cryptography.SHA256]::Create().ComputeHash([System.Text.Encoding]::UTF8.GetBytes([guid]::NewGuid().ToString()))) | Substring(0,32)
```

---

## Paso 6: Configurar Dockerfile (Opcional)

Si Railway no detecta automáticamente el Dockerfile:

1. En la pestaña **Settings** de tu servicio
2. Ve a **Build** o **Dockerfile**
3. Asegúrate de que apunta a: `./Dockerfile`
4. Railway ejecutará automáticamente las migraciones gracias al script en el Dockerfile

---

## Paso 7: Desplegar la Aplicación

### Opción A: Despliegue Automático (Desde GitHub)

1. Railway detectará automáticamente cambios en GitHub
2. Cuando hagas un `git push` a `main`, se desplegará automáticamente
3. Monitorea el despliegue en tiempo real en la consola

### Opción B: Despliegue Manual

```bash
# Usando Railway CLI
railway up
```

---

## Paso 8: Monitorear el Despliegue

### En la UI de Railway:

1. Ve a tu proyecto
2. Haz clic en el servicio **API**
3. Abre la pestaña **Logs**
4. Observa:

```
✅ 🔄 Iniciando migración de base de datos...
✅ ✅ Migraciones aplicadas exitosamente
✅ No se encontraron juegos en la BD. Iniciando seeding desde Steam dataset...
✅ ✅ Seeding de juegos completado exitosamente
✅ Application started. Press Ctrl+C to shut down.
```

### Verificar Health Check:

```bash
curl https://tu-dominio-railway.railway.app/health
```

**Respuesta esperada (200 OK):**
```json
{
  "status": "Healthy",
  "checks": {
	"db": {
	  "status": "Healthy"
	}
  }
}
```

---

## Paso 9: Obtener URL de Producción

1. En la pestaña **Deployments** de tu servicio API
2. Verás una URL única asignada por Railway
3. Ejemplo: `https://nuevoforo-production-xxxx.railway.app`

### Acceder a Swagger (Solo en Desarrollo)

Si deseas acceder a Swagger en producción, modifica `Program.cs`:

```csharp
if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
```

Luego accede a: `https://tu-dominio/swagger/index.html`

---

## Paso 10: Testear Endpoints

### Con cURL:

```bash
# 1. Register
curl -X POST https://tu-dominio/auth/register \
  -H "Content-Type: application/json" \
  -d '{
	"email": "test@example.com",
	"password": "Password123!",
	"userName": "testuser"
  }'

# 2. Login
curl -X POST https://tu-dominio/auth/login \
  -H "Content-Type: application/json" \
  -d '{
	"email": "test@example.com",
	"password": "Password123!"
  }'

# 3. Get Games (con JWT token)
curl -X GET https://tu-dominio/games \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# 4. Health Check
curl -X GET https://tu-dominio/health
```

### Con Postman:

1. Importa los endpoints desde `src/NuevoForo.Api/NuevoForo.Api.http`
2. Reemplaza `localhost:5001` con tu URL de Railway
3. Copia el JWT token de la respuesta de login
4. Agrega en Headers: `Authorization: Bearer <token>`

---

## Solución de Problemas

### Error: "Database migration failed"

```
❌ Error al aplicar migraciones de base de datos
```

**Soluciones:**
1. Verifica que PostgreSQL esté configurado correctamente
2. Revisa que `CONNECTION_STRING` sea válida
3. Comprueba los logs completos: `railway logs`

### Error: "Port already in use"

Railway asigna automáticamente el puerto. El Dockerfile ya está configurado:
```dockerfile
EXPOSE 8080
ASPNETCORE_URLS=http://+:8080
```

### Error: "Connection timeout"

1. Verifica que PostgreSQL esté iniciado
2. Revisa firewall/networking de Railway
3. Intenta reconectar el servicio PostgreSQL

### Logs vacíos o no se ven cambios

```bash
# Ver logs recientes
railway logs --service=<nombre-api>

# Ver logs en tiempo real
railway logs -f
```

---

## Próximos Pasos

✅ Aplicación desplegada en Railway  
✅ Base de datos PostgreSQL funcional  
✅ Migraciones automáticas ejecutadas  
✅ Seeds de juegos cargados  

### Para Producción:

1. **Configurar dominio personalizado**
   - Railway > Project > Domains > + Add Domain

2. **Agregar certificado SSL/TLS**
   - Railway lo proporciona automáticamente

3. **Configurar backups de BD**
   - Railway > PostgreSQL > Backups

4. **Monitorear métricas**
   - Railway > Metrics (CPU, Memory, Network)

5. **Configurar alertas**
   - Railway > Alerts

---

## Información Útil

**Dashboard:** https://railway.app/dashboard  
**Documentación:** https://docs.railway.app  
**CLI GitHub:** https://github.com/railwayapp/cli  
**Status Page:** https://status.railway.app
