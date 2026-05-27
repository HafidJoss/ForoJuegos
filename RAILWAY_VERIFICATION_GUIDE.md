# 🧪 Guía de Verificación Post-Despliegue en Railway

## 📌 Información Necesaria

Después del despliegue, tendrás esta información de Railway:

```
URL Pública: https://[tu-proyecto]-production-xxxx.railway.app
PostgreSQL Host: [host].railway.app
PostgreSQL Port: 5432
PostgreSQL Database: railway
PostgreSQL User: postgres
PostgreSQL Password: [generado por Railway]
```

---

## 1️⃣ Verificación de Health Checks

### Endpoint: `/health`

```bash
curl -i https://tu-dominio-railway.app/health
```

**Respuesta esperada (200 OK):**
```json
{
  "status": "Healthy",
  "checks": {
	"db": {
	  "status": "Healthy",
	  "duration": "00:00:00.1234567",
	  "exception": null,
	  "tags": ["db"],
	  "data": {
		"timeout": "00:00:00"
	  }
	}
  },
  "totalDuration": "00:00:00.1234567"
}
```

**Si falla:**
- [ ] Verifica conexión PostgreSQL
- [ ] Revisa variables de entorno
- [ ] Mira logs: `railway logs -f`

### Endpoint: `/health/db`

```bash
curl -i https://tu-dominio-railway.app/health/db
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

## 2️⃣ Verificación de Migraciones

### Ver Logs de Migraciones

```bash
railway logs | grep -i "migración"
```

**Salida esperada:**
```
🔄 Iniciando migración de base de datos...
✅ Migraciones aplicadas exitosamente
```

### Verificar Tablas en BD

```bash
# Acceder a PostgreSQL en Railway
railway connect --service postgres

# En psql:
\dt  # Lista todas las tablas

# Deberías ver:
# - usuarios
# - juegos
# - resenas
# - comentarios
# - contenido_ugc
# - likes_resena
# - likes_ugc
# - reportes
# - notificaciones
# - donaciones
```

---

## 3️⃣ Verificación de Seeds

### Ver Logs de Seeding

```bash
railway logs | grep -i "seeding\|juego"
```

**Salida esperada:**
```
No se encontraron juegos en la BD. Iniciando seeding desde Steam dataset...
✅ Seeding de juegos completado exitosamente: {resumen}
✅ BD ya contiene N juegos. Seeding omitido.
```

### Contar Juegos en BD

```bash
# En psql (desde railway connect):
SELECT COUNT(*) FROM juegos;

# Deberías ver:
# count
# -------
#   200+ (según steam_games_sample.json)
```

---

## 4️⃣ Testing de Autenticación

### 1. Registrar Usuario

```bash
curl -X POST https://tu-dominio-railway.app/auth/register \
  -H "Content-Type: application/json" \
  -d '{
	"email": "testuser@example.com",
	"password": "TestPassword123!",
	"userName": "testuser"
  }'
```

**Respuesta esperada (200 OK):**
```json
{
  "message": "Usuario registrado exitosamente",
  "user": {
	"id": "uuid",
	"email": "testuser@example.com",
	"userName": "testuser"
  }
}
```

### 2. Login y Obtener Token JWT

```bash
curl -X POST https://tu-dominio-railway.app/auth/login \
  -H "Content-Type: application/json" \
  -d '{
	"email": "testuser@example.com",
	"password": "TestPassword123!"
  }'
```

**Respuesta esperada (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "...",
  "user": {
	"id": "uuid",
	"email": "testuser@example.com",
	"userName": "testuser"
  }
}
```

**Guardar el token para los próximos tests:**
```bash
export TOKEN="tu_jwt_token_aqui"
```

---

## 5️⃣ Testing de Endpoints Principales

### GET /games - Listar Juegos

```bash
curl -X GET "https://tu-dominio-railway.app/games?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer $TOKEN"
```

**Respuesta esperada (200 OK):**
```json
{
  "data": [
	{
	  "id": "uuid",
	  "nombre": "Dota 2",
	  "descripcion": "...",
	  "generoPrincipal": "Strategy",
	  "imagenPortadaUrl": "https://...",
	  "fechaLanzamiento": "2013-07-09"
	}
	// ... más juegos
  ],
  "totalCount": 200+,
  "pageNumber": 1,
  "pageSize": 10
}
```

### POST /reviews - Crear Reseña

```bash
curl -X POST https://tu-dominio-railway.app/reviews \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
	"juegoId": "uuid_de_un_juego",
	"titulo": "Excelente juego",
	"contenido": "Me encantó este juego",
	"puntuacion": 9
  }'
```

**Respuesta esperada (201 Created):**
```json
{
  "id": "uuid",
  "titulo": "Excelente juego",
  "contenido": "Me encantó este juego",
  "puntuacion": 9,
  "estado": "Aprobado",
  "fechaCreacion": "2026-05-27T12:34:56"
}
```

### GET /reviews - Listar Reseñas

```bash
curl -X GET "https://tu-dominio-railway.app/reviews?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer $TOKEN"
```

---

## 6️⃣ Testing sin Autenticación (Debe Fallar)

### Intentar acceder a endpoint protegido sin token

```bash
curl -X GET https://tu-dominio-railway.app/games
```

**Respuesta esperada (401 Unauthorized):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authorization header is missing or invalid."
}
```

---

## 7️⃣ Testing de Rate Limiting

### Enviar múltiples requests rápidamente

```bash
for i in {1..15}; do
  echo "Request $i"
  curl -X GET "https://tu-dominio-railway.app/health" \
	-w "Status: %{http_code}\n"
  sleep 0.1
done
```

**Resultado esperado:**
- Primeros 10 requests: 200 OK (límite global: 100 por minuto)
- Después: 429 Too Many Requests (si excedes límite)

---

## 8️⃣ Testing desde Postman

### Importar Collection

1. Abre Postman
2. File > Import
3. Selecciona `src/NuevoForo.Api/NuevoForo.Api.http`
4. Reemplaza `localhost:5001` con tu URL de Railway
5. Ejecuta los requests

### Variables de Entorno en Postman

```json
{
  "api_base_url": "https://tu-dominio-railway.app",
  "jwt_token": "tu_token_aqui",
  "user_email": "testuser@example.com",
  "user_password": "TestPassword123!"
}
```

---

## 9️⃣ Verificación de Logs en Tiempo Real

### Ver todos los logs

```bash
railway logs
```

### Ver logs en vivo (streaming)

```bash
railway logs -f
```

### Filtrar logs por servicio

```bash
# API logs
railway logs --service NuevoForo

# PostgreSQL logs
railway logs --service postgres
```

### Buscar errors

```bash
railway logs | grep -i "error\|exception\|failed"
```

---

## 🔟 Verificación de Variables de Entorno

### Ver todas las variables configuradas

```bash
railway variable list
```

**Debería mostrar:**
```
ConnectionStrings__DefaultConnection ****
Jwt__Issuer NuevoForo
Jwt__Audience NuevoForo
Jwt__SigningKey ****
... (más variables)
```

### Verificar variables específicas

```bash
railway variable get Jwt__Issuer
railway variable get ASPNETCORE_ENVIRONMENT
```

---

## 1️⃣1️⃣ Verificación de Recursos

### Ver uso de CPU y Memoria

```bash
railway status
```

**Salida esperada:**
```
Project: NuevoForo
Services:
  - API: Running (CPU: 5-10%, Memory: 128-256MB)
  - PostgreSQL: Running (CPU: 2-5%, Memory: 256-512MB)
```

---

## 1️⃣2️⃣ Verificación CORS (si tienes Frontend)

### Desde tu frontend local

```javascript
// JavaScript
fetch('https://tu-dominio-railway.app/games', {
  headers: {
	'Authorization': 'Bearer ' + token
  }
})
.then(r => r.json())
.then(data => console.log(data))
```

**Deberías ver:**
- 200 OK con datos de juegos
- Sin errores de CORS
- Validación de JWT correcta

---

## 🔴 Solución de Problemas Comunes

### ❌ "Connection refused"
**Causa:** PostgreSQL no está disponible  
**Solución:**
```bash
railway logs | grep -i "postgres\|connection"
# Reconectar PostgreSQL en Railway
```

### ❌ "Authentication failed"
**Causa:** Variables JWT no configuradas  
**Solución:**
```bash
railway variable list | grep Jwt
# Verificar que todas las variables Jwt existan
```

### ❌ "Migration timeout"
**Causa:** BD muy lenta o demasiados datos  
**Solución:**
```bash
# Ver logs de migración
railway logs | grep -i "migración"
# Esperar o reconectar
```

### ❌ "Disk space exceeded"
**Causa:** Too much data in uploads  
**Solución:**
```bash
# Railway notificará automáticamente
# Limpia archivos UGC antiguos
# O aumenta tier del plan
```

---

## ✅ Checklist de Verificación Exitosa

- [ ] Health check responde 200 OK
- [ ] BD Health check responde 200 OK
- [ ] Migraciones completadas exitosamente
- [ ] Seeds de juegos cargados
- [ ] Usuario se puede registrar
- [ ] Usuario se puede autenticar
- [ ] JWT token válido
- [ ] GET /games retorna juegos
- [ ] POST /reviews funciona
- [ ] Rate limiting está activo
- [ ] Sin autenticación retorna 401
- [ ] Logs muestran actividad normal
- [ ] URL pública accesible

---

## 📊 URLs de Monitoreo

| Recurso | URL |
|---------|-----|
| Dashboard | https://railway.app/dashboard |
| Logs API | Railway CLI: `railway logs` |
| Status | https://status.railway.app |
| Documentación | https://docs.railway.app |

---

## 🎉 ¡Despliegue Verificado!

Si todos los checks pasaron, tu aplicación NuevoForo está funcionando correctamente en Railway.

**Próximos pasos:**
1. Configurar dominio personalizado
2. Agregar más datos/usuarios
3. Monitorear logs regularmente
4. Optimizar según necesidad

**Soporte:**
- Railway Docs: https://docs.railway.app
- Email: support@railway.app
- Discord: https://discord.gg/railway
