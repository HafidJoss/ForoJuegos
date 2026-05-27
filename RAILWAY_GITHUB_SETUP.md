# 🚀 Guía: Configurar Git y GitHub para Railway

## Opción A: Si No Tienes Git Inicializado (Recomendado)

### Paso 1: Inicializar repositorio local
```bash
cd C:\Users\PC\source\repos\NuevoForo\
git init
git add .
git commit -m "Inicial: Configuración para despliegue en Railway

- Dockerfile multi-stage optimizado para .NET 10
- Migraciones automáticas de BD en Program.cs
- railway.json con configuración de despliegue
- .gitignore y .env.example para gestión segura"
```

### Paso 2: Crear repositorio en GitHub

1. Ve a https://github.com/new
2. Completa los detalles:
   - **Repository name:** `NuevoForo`
   - **Description:** `Foro de videojuegos con reseñas, comunidad y moderación`
   - **Visibility:** Private (recomendado) o Public según prefieras
   - **Add .gitignore:** No (ya lo tenemos)
   - **Add a LICENSE:** Selecciona MIT (opcional)
   - **No inicialices con README**

3. Clic en **Create repository**

### Paso 3: Conectar repositorio remoto
```bash
git remote add origin https://github.com/TU_USUARIO/NuevoForo.git
git branch -M main
git push -u origin main
```

**Nota:** Reemplaza `TU_USUARIO` con tu nombre de usuario de GitHub.

---

## Opción B: Si Ya Tienes Git Inicializado

### Verificar estado actual
```bash
git status
git log --oneline -5
```

### Agregar nuevos archivos para Railway
```bash
git add Dockerfile .dockerignore railway.json .env.example .gitignore
git commit -m "Agregar configuración para Railway

- Dockerfile multi-stage optimizado para .NET 10
- railway.json con health check
- Migraciones automáticas en Program.cs"
```

### Pushear cambios
```bash
git push origin main
```

---

## Opción C: Desplegar sin GitHub (ZIP Upload)

Si prefieres no usar GitHub, Railway permite ZIP upload directo:

```bash
# Crear archivo ZIP con el proyecto
# En PowerShell:
Compress-Archive -Path "C:\Users\PC\source\repos\NuevoForo\*" -DestinationPath "C:\Users\PC\NuevoForo.zip" -Force

# Luego en Railway, selecciona "Upload from ZIP"
```

---

## Verificación Post-Push

Verifica que tus archivos estén en GitHub:

```bash
# Ver rama actual
git branch

# Ver archivos
git ls-files | grep -E "(Dockerfile|railway.json|.env.example)"

# Ver histórico
git log --oneline -10
```

**Resultado esperado:**
```
Dockerfile
.dockerignore
railway.json
.env.example
.gitignore
Program.cs (modificado)
```

---

## 📝 Próximo Paso

Una vez confirmado que está en GitHub, procede a:
1. Crear cuenta en Railway (https://railway.app)
2. Crear nuevo proyecto
3. Conectar con GitHub o subir ZIP
4. Agregar servicio de PostgreSQL
5. Configurar variables de entorno
