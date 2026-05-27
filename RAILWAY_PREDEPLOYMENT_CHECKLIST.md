# ✅ Checklist Pre-Despliegue en Railway

## 📋 Verificación Local

### Código y Git
- [ ] El proyecto compila sin errores: `dotnet build -c Release`
- [ ] No hay cambios sin commitear: `git status`
- [ ] Todos los commits están listos: `git log --oneline -5`
- [ ] Remote GitHub está configurado: `git remote -v`
- [ ] Rama principal es "main": `git rev-parse --abbrev-ref HEAD`

### Base de Datos
- [ ] Migraciones locales se aplican correctamente: `dotnet ef database update -p src/NuevoForo.Infrastructure`
- [ ] Seed de juegos funciona localmente
- [ ] No hay errores de conexión a BD local

### Aplicación
- [ ] API se ejecuta correctamente: `dotnet run -p src/NuevoForo.Api`
- [ ] Swagger está disponible en `https://localhost:5001/swagger`
- [ ] `/health` endpoint responde 200 OK
- [ ] `/health/db` endpoint responde 200 OK
- [ ] Tests unitarios pasan: `dotnet test`

### Archivos Necesarios
- [ ] `Dockerfile` existe y está bien formado
- [ ] `.dockerignore` existe
- [ ] `railway.json` existe
- [ ] `.gitignore` existe
- [ ] `.env.example` existe
- [ ] `Program.cs` incluye migraciones automáticas

---

## 🌐 Configuración en Railway

### Preparación
- [ ] Cuenta de Railway creada
- [ ] GitHub conectado (opcional pero recomendado)
- [ ] Railway CLI instalado: `npm install -g @railway/cli`

### Servicios
- [ ] PostgreSQL agregado al proyecto
- [ ] Variables de PostgreSQL son accesibles
- [ ] DATABASE_URL generada correctamente
- [ ] Conexión a BD probada desde Railway

### Variables de Entorno
- [ ] `ConnectionStrings__DefaultConnection` configurada
- [ ] `Jwt__Issuer` configurado
- [ ] `Jwt__Audience` configurado
- [ ] `Jwt__SigningKey` es segura (32+ caracteres)
- [ ] `Jwt__ExpirationMinutes` configurado (ej: 60)
- [ ] `RateLimiting__Global__PermitLimit` configurado
- [ ] `RateLimiting__Global__WindowSeconds` configurado
- [ ] `RateLimiting__Auth__PermitLimit` configurado
- [ ] `RateLimiting__Auth__WindowSeconds` configurado
- [ ] `ASPNETCORE_ENVIRONMENT=Production` configurado
- [ ] `ASPNETCORE_URLS=http://+:8080` configurado

---

## 🚀 Procedimiento de Despliegue

### Opción A: Script Automático (Recomendado para Windows)

```powershell
# Ejecutar script con automático
.\deploy.ps1 -AutoDeploy -CommitMessage "Deploy a Railway"

# O interactivo
.\deploy.ps1
```

### Opción B: Despliegue Manual

```bash
# 1. Commit y Push
git add .
git commit -m "Listo para despliegue en Railway"
git push -u origin main

# 2. Railway CLI
railway login
railway up

# 3. Monitorear
railway logs -f
```

---

## 📊 Monitoreo Post-Despliegue

### Verificación Inmediata
- [ ] Despliegue completado sin errores en Railway dashboard
- [ ] Logs muestran: "✅ Migraciones aplicadas exitosamente"
- [ ] Logs muestran: "✅ Seeding de juegos completado exitosamente"
- [ ] API inicia sin excepciones
- [ ] Health check responde 200 OK

### Primeros Minutos
- [ ] Acceder a `/health` desde URL pública
- [ ] Acceder a `/health/db` desde URL pública
- [ ] Verificar que la BD está poblada
- [ ] Ver logs en tiempo real: `railway logs -f`

### Testing de Endpoints
- [ ] POST `/auth/register` - Crear usuario
- [ ] POST `/auth/login` - Obtener JWT token
- [ ] GET `/games` - Con JWT token
- [ ] GET `/health` - Sin autenticación
- [ ] Verificar CORS desde frontend local si aplica

---

## 🔍 Solución Rápida de Problemas

### Problema: Migraciones fallan
**Solución:**
1. Verifica conexión BD: `railway db status`
2. Ver logs: `railway logs -f`
3. Reconectar PostgreSQL en Railway
4. Redeploy

### Problema: Aplicación no inicia
**Solución:**
1. Ver logs completos: `railway logs`
2. Verificar variables de entorno
3. Compilar localmente: `dotnet build`
4. Revisar Dockerfile

### Problema: Port no disponible
**Ya está manejado.** Railway asigna automáticamente PORT, y el Dockerfile lo usa.

### Problema: BD vacía después del despliegue
**Solución:**
1. Verifica que seeding esté habilitado
2. Ver logs de seeding
3. Ejecutar manualmente si es necesario
4. Ver base de datos desde Railway console

---

## 📈 Post-Despliegue (Próximas Horas/Días)

- [ ] Configurar dominio personalizado (opcional)
- [ ] Configurar SSL/TLS (Railway lo incluye)
- [ ] Agregar región preferida
- [ ] Configurar backups automáticos de BD
- [ ] Configurar alertas
- [ ] Documentar URL pública
- [ ] Compartir URL con equipo
- [ ] Probar desde dispositivos reales/externos
- [ ] Verificar logs regularmente
- [ ] Revisar uso de recursos (CPU, memoria)

---

## 📝 Notas Importantes

1. **Seguridad:**
   - Nunca commits claves JWT en repositorio
   - Usa `.env.example` como template
   - Cambia `Jwt__SigningKey` en producción

2. **Base de Datos:**
   - Railway crea automáticamente backups
   - Las migraciones se ejecutan automáticamente
   - Los seeds se ejecutan solo si la BD está vacía

3. **Reintentos:**
   - Si el despliegue falla, Railway reinicia automáticamente
   - Verifica logs antes de reintentar manualmente

4. **Contacto:**
   - Soporte Railway: https://railway.app/support
   - Documentación: https://docs.railway.app
   - Status: https://status.railway.app

---

## ✅ Lista de Verificación Final

Antes de dar por completado el despliegue:

- [ ] Aplicación responde en URL pública
- [ ] Todos los endpoints principales funcionan
- [ ] Base de datos está conectada y tiene datos
- [ ] Migraciones se ejecutaron sin errores
- [ ] Logs muestran actividad normal
- [ ] No hay errores de conexión
- [ ] CORS funciona si tienes frontend
- [ ] JWT tokens se generan correctamente
- [ ] Rate limiting está activo
- [ ] Health checks responden OK

---

## 🎉 ¡Completado!

Felicidades, tu aplicación NuevoForo está en producción en Railway.

**URLs útiles:**
- Dashboard: https://railway.app/dashboard
- Logs: `railway logs -f`
- Status: `railway status`
- Proyecto: `railway link`

**Próximos pasos:**
1. Monitorear regularmente los logs
2. Agregar más usuarios de prueba
3. Optimizar si es necesario
4. Configurar CI/CD adicional
5. Documentar procesos internos
