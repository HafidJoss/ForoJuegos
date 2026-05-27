# 🚀 Despliegue Completo de NuevoForo en Railway

**Status:** ✅ Completado y Listo para Usar

---

## 📚 ¿Por Dónde Empiezo?

### 🎯 Opción 1: Despliegue Rápido (5-10 minutos)

**Para Windows (Recomendado):**
```powershell
.\deploy.ps1 -AutoDeploy
```

**Para Linux/Mac:**
```bash
chmod +x deploy.sh
./deploy.sh
```

---

### 📖 Opción 2: Guía Paso a Paso Completo

1. **Primero:** Lee [DEPLOYMENT_SUMMARY.md](./DEPLOYMENT_SUMMARY.md)
   - Resumen ejecutivo de todo
   - Checklist de verificación

2. **GitHub Setup:** Lee [RAILWAY_GITHUB_SETUP.md](./RAILWAY_GITHUB_SETUP.md)
   - Cómo conectar GitHub
   - O hacer ZIP upload si prefieres

3. **Despliegue en Railway:** Lee [RAILWAY_DEPLOYMENT_GUIDE.md](./RAILWAY_DEPLOYMENT_GUIDE.md)
   - Paso a paso en Railway UI
   - Configuración de variables
   - PostgreSQL setup

4. **Verificación:** Lee [RAILWAY_VERIFICATION_GUIDE.md](./RAILWAY_VERIFICATION_GUIDE.md)
   - Cómo testear después de desplegar
   - Verificar endpoints
   - Solución de problemas

5. **Pre-Despliegue:** Usa [RAILWAY_PREDEPLOYMENT_CHECKLIST.md](./RAILWAY_PREDEPLOYMENT_CHECKLIST.md)
   - Checklist de verificación
   - Asegurar que todo está listo

---

## 🔍 ¿Qué Se Ha Configurado?

### ✅ Archivos Creados

```
Dockerfile                              Contenedor multi-stage para .NET 10
.dockerignore                           Optimización de imagen
railway.json                            Configuración de Railway
.env.example                            Template de variables de entorno
.gitignore                              Archivos a ignorar en Git
deploy.ps1                              Script PowerShell (Windows)
deploy.sh                               Script Bash (Linux/Mac)

DEPLOYMENT_SUMMARY.md                   Resumen ejecutivo
RAILWAY_DEPLOYMENT_GUIDE.md             Guía paso a paso
RAILWAY_GITHUB_SETUP.md                 Setup de GitHub
RAILWAY_PREDEPLOYMENT_CHECKLIST.md      Checklist
RAILWAY_VERIFICATION_GUIDE.md           Testing y validación
```

### ✅ Cambios en Código

**src/NuevoForo.Api/Program.cs:**
- Agregado: `using Microsoft.EntityFrameworkCore;`
- Agregado: Bloque de migraciones automáticas
- Agregado: `await dbContext.Database.MigrateAsync();`
- Beneficio: Las BD se migran automáticamente al iniciar

---

## 📊 Flujo de Despliegue

```
Tu código
	↓
git push (GitHub)
	↓
Railway detecta cambios
	↓
Docker build (multi-stage)
	↓
Migraciones automáticas ✨
	↓
Seeds de datos
	↓
API en producción
	↓
✅ Aplicación disponible
```

---

## 🚀 Comandos Rápidos

### Opción 1: Script Automático (Recomendado)

**Windows:**
```powershell
.\deploy.ps1 -AutoDeploy -CommitMessage "Deploy a Railway"
```

**Linux/Mac:**
```bash
./deploy.sh
```

### Opción 2: Despliegue Manual

```bash
# 1. Git
git add .
git commit -m "Deploy a Railway"
git push -u origin main

# 2. Railway CLI
npm install -g @railway/cli
railway login
railway up

# 3. Monitorear
railway logs -f
```

### Opción 3: ZIP Upload (Sin GitHub)

```bash
# Crear ZIP
Compress-Archive -Path "C:\Users\PC\source\repos\NuevoForo\*" `
  -DestinationPath "C:\Users\PC\NuevoForo.zip"

# Luego en Railway UI: "Upload from ZIP"
```

---

## 📋 Requisitos Previos

- [ ] .NET 10 SDK instalado
- [ ] Git instalado
- [ ] GitHub (opcional, si usas git push)
- [ ] Railway CLI (opcional, si usas `railway up`)

### Instalar si no tienes:

```bash
# Railway CLI
npm install -g @railway/cli

# O si usas PowerShell
npm install -g @railway/cli

# Verificar
railway --version
```

---

## 🎯 Paso a Paso Rápido (Sin Scripts)

### 1. Verificación Local

```bash
# Compilar
dotnet build -c Release

# Inicializar Git
git init
git add .
git commit -m "Inicial"
```

### 2. GitHub (Opcional)

```bash
# Crear repositorio en GitHub primero

# Conectar
git remote add origin https://github.com/TU_USUARIO/NuevoForo.git
git branch -M main
git push -u origin main
```

### 3. Railway

```bash
# 1. Crear cuenta en https://railway.app
# 2. Nuevo proyecto > Deploy from GitHub
# 3. Conectar repositorio
# 4. Agregar PostgreSQL
# 5. Configurar variables de entorno

# O con CLI:
railway login
railway up
```

### 4. Verificar

```bash
# Ver status
railway status

# Ver logs
railway logs -f

# Probar health check
curl https://tu-dominio-railway.app/health
```

---

## ⚙️ Configuración de Variables de Entorno en Railway

Una vez en el dashboard de Railway, agregar estas variables:

```
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/railway
Jwt__Issuer=NuevoForo
Jwt__Audience=NuevoForo
Jwt__SigningKey=CLAVE_SEGURA_DE_32_CARACTERES_MINIMO
Jwt__ExpirationMinutes=60
RateLimiting__Global__PermitLimit=100
RateLimiting__Global__WindowSeconds=60
RateLimiting__Auth__PermitLimit=10
RateLimiting__Auth__WindowSeconds=60
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
```

**Generar JWT Key segura:**
```bash
# PowerShell
[Convert]::ToBase64String([Security.Cryptography.SHA256]::Create().ComputeHash([Text.Encoding]::UTF8.GetBytes([guid]::NewGuid().ToString()))) -Substring 0,32

# Bash
echo -n "$(openssl rand -base64 32)" | head -c 32
```

---

## 🧪 Testing Post-Despliegue

### Health Check

```bash
curl https://tu-dominio-railway.app/health
```

**Resultado esperado:** 200 OK + JSON

### Crear Usuario

```bash
curl -X POST https://tu-dominio-railway.app/auth/register \
  -H "Content-Type: application/json" \
  -d '{
	"email": "test@example.com",
	"password": "TestPassword123!",
	"userName": "testuser"
  }'
```

### Login

```bash
curl -X POST https://tu-dominio-railway.app/auth/login \
  -H "Content-Type: application/json" \
  -d '{
	"email": "test@example.com",
	"password": "TestPassword123!"
  }'
```

**Resultado:** Token JWT

### Ver Juegos

```bash
curl https://tu-dominio-railway.app/games \
  -H "Authorization: Bearer <JWT_TOKEN>"
```

**Resultado:** Lista de juegos

---

## 📖 Documentación Completa

| Documento | Para Qué |
|-----------|----------|
| **DEPLOYMENT_SUMMARY.md** | Resumen de todo el proceso |
| **RAILWAY_GITHUB_SETUP.md** | Configurar Git/GitHub |
| **RAILWAY_DEPLOYMENT_GUIDE.md** | Guía paso a paso Railway |
| **RAILWAY_PREDEPLOYMENT_CHECKLIST.md** | Verificaciones antes de desplegar |
| **RAILWAY_VERIFICATION_GUIDE.md** | Testing después de desplegar |

---

## 🆘 Solución Rápida de Problemas

### "Docker build fails"
- Ver logs: `railway logs`
- Verificar Dockerfile: `cat Dockerfile`
- Compilar localmente: `dotnet build`

### "Migration failed"
- Verificar BD: `railway connect --service postgres`
- Ver logs: `railway logs | grep -i migración`
- Reconectar PostgreSQL

### "Auth not working"
- Verificar JWT key: `railway variable get Jwt__SigningKey`
- Verificar JWT Issuer/Audience
- Ver logs de autenticación

### "Can't access API"
- Verificar URL: `railway link`
- Verificar health: `/health`
- Verificar firewall Railway

Ver más en: **RAILWAY_VERIFICATION_GUIDE.md**

---

## 📞 Recursos Útiles

- **Railway Docs:** https://docs.railway.app
- **Railway Status:** https://status.railway.app
- **Railway Discord:** https://discord.gg/railway
- **Este Proyecto:** Consulta SYSTEM_OVERVIEW.md para arquitectura

---

## ✅ Checklist Final

Antes de desplegar:

- [ ] `dotnet build -c Release` sin errores
- [ ] Git inicializado: `git status`
- [ ] Todos los archivos listos (Dockerfile, etc.)
- [ ] Railway CLI instalado (opcional)
- [ ] Railway cuenta creada
- [ ] PostgreSQL agregado en Railway
- [ ] Variables de entorno listadas

Después de desplegar:

- [ ] Health check OK: `/health`
- [ ] BD Health check OK: `/health/db`
- [ ] Usuario se puede registrar
- [ ] Autenticación funciona
- [ ] Endpoints retornan datos
- [ ] Logs muestran info normal

---

## 🎉 ¡Listo!

Tu aplicación **NuevoForo** está lista para ser desplegada en Railway.

**Siguiente paso:**

```powershell
# Windows
.\deploy.ps1 -AutoDeploy

# Linux/Mac
./deploy.sh

# O GitHub push
git push origin main
```

---

**Preguntas?** Revisa la documentación en los archivos .md incluidos.

**¡Éxito con tu despliegue!** 🚀
