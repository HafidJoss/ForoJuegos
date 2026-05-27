# PowerShell Script de Despliegue Rápido en Railway
# Ejecutar: .\deploy.ps1

param(
	[switch]$AutoDeploy = $false,
	[string]$CommitMessage = "Update"
)

# Funciones auxiliares
function Write-Success {
	param([string]$Message)
	Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Info {
	param([string]$Message)
	Write-Host "ℹ️ $Message" -ForegroundColor Blue
}

function Write-Error-Custom {
	param([string]$Message)
	Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Header {
	param([string]$Message)
	Write-Host "`n========================================" -ForegroundColor Blue
	Write-Host "🚀 $Message" -ForegroundColor Blue
	Write-Host "========================================`n" -ForegroundColor Blue
}

# Script Principal
Write-Header "Script de Despliegue en Railway"

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "NuevoForo.slnx")) {
	Write-Error-Custom "No se encontró NuevoForo.slnx"
	Write-Info "Ejecuta este script desde la raíz del proyecto"
	exit 1
}

# Paso 1: Verificar Git
Write-Info "Paso 1: Verificando Git..."
try {
	$gitStatus = git status 2>&1
	Write-Success "Git inicializado"
} catch {
	Write-Error-Custom "Git no está inicializado"
	exit 1
}

# Paso 2: Verificar cambios sin commitear
Write-Info "Paso 2: Verificando cambios pendientes..."
$changes = git status --short
if ($changes) {
	Write-Host "Cambios detectados:" -ForegroundColor Blue
	Write-Host $changes

	if (-not $AutoDeploy) {
		$response = Read-Host "`n¿Deseas commitear estos cambios? (s/n)"
		if ($response -eq "s" -or $response -eq "S") {
			git add .
			git commit -m $CommitMessage
			Write-Success "Cambios commiteados"
		}
	} else {
		git add .
		git commit -m $CommitMessage
		Write-Success "Cambios commiteados (modo automático)"
	}
}

# Paso 3: Verificar rama main
Write-Info "Paso 3: Verificando rama..."
$currentBranch = git rev-parse --abbrev-ref HEAD
Write-Host "Rama actual: $currentBranch" -ForegroundColor Blue

if ($currentBranch -ne "main") {
	if (-not $AutoDeploy) {
		$response = Read-Host "¿Deseas cambiar a rama 'main'? (s/n)"
		if ($response -eq "s" -or $response -eq "S") {
			git branch -M main
			Write-Success "Rama renombrada a 'main'"
		}
	} else {
		git branch -M main
		Write-Success "Rama renombrada a 'main' (modo automático)"
	}
}

# Paso 4: Compilar el proyecto
Write-Info "Paso 4: Compilando proyecto..."
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
	Write-Error-Custom "Error en compilación"
	exit 1
}
Write-Success "Compilación exitosa"

# Paso 5: Push a GitHub
Write-Info "Paso 5: Pusheando cambios a GitHub..."
$remote = git remote get-url origin 2>$null
if (-not $remote) {
	Write-Error-Custom "No hay remote configurado"
	Write-Info "Agrega GitHub como remote:"
	Write-Host "git remote add origin <URL_DE_TU_REPO_GITHUB>" -ForegroundColor Yellow
	exit 1
}

Write-Host "Remote GitHub: $remote" -ForegroundColor Blue

if (-not $AutoDeploy) {
	$response = Read-Host "¿Deseas pushear a GitHub? (s/n)"
} else {
	$response = "s"
}

if ($response -eq "s" -or $response -eq "S") {
	git push -u origin main
	if ($LASTEXITCODE -ne 0) {
		Write-Error-Custom "Error al pushear"
		exit 1
	}
	Write-Success "Push exitoso"
}

# Paso 6: Verificar CLI de Railway
Write-Info "Paso 6: Verificando Railway CLI..."
$railwayInstalled = $null -ne (Get-Command railway -ErrorAction SilentlyContinue)

if (-not $railwayInstalled) {
	Write-Error-Custom "Railway CLI no está instalado"
	Write-Info "Instálalo con: npm install -g @railway/cli"
	Write-Host "Luego ejecuta de nuevo este script" -ForegroundColor Yellow
} else {
	Write-Success "Railway CLI instalado"

	# Paso 7: Login en Railway
	Write-Info "Paso 7: Verificando autenticación en Railway..."
	try {
		railway status > $null 2>&1
		Write-Success "Autenticado en Railway"
	} catch {
		Write-Info "Necesitas autenticarte en Railway"
		railway login
	}

	# Paso 8: Desplegar
	Write-Info "Paso 8: Desplegando en Railway..."

	if (-not $AutoDeploy) {
		$response = Read-Host "¿Deseas desplegar ahora? (s/n)"
	} else {
		$response = "s"
	}

	if ($response -eq "s" -or $response -eq "S") {
		Write-Host "Iniciando despliegue..." -ForegroundColor Yellow
		railway up

		if ($LASTEXITCODE -eq 0) {
			Write-Success "Despliegue enviado a Railway"
			Write-Host "`nPróximos pasos:" -ForegroundColor Blue
			Write-Host "1. Ve a https://railway.app/dashboard" -ForegroundColor Cyan
			Write-Host "2. Verifica los logs del despliegue" -ForegroundColor Cyan
			Write-Host "3. Espera a que las migraciones se completen" -ForegroundColor Cyan
			Write-Host "4. Accede a tu aplicación en la URL proporcionada" -ForegroundColor Cyan
		}
	}
}

Write-Header "✅ Script completado"

# Información adicional
Write-Info "Para más información, consulta RAILWAY_DEPLOYMENT_GUIDE.md"
Write-Info "Logs en tiempo real: railway logs -f"
Write-Info "Estado del proyecto: railway status"
