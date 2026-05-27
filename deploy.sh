#!/bin/bash
# Script de Despliegue Rápido en Railway

# Colores para output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}🚀 Script de Despliegue en Railway${NC}"
echo -e "${BLUE}========================================${NC}"

# Verificar que estamos en el directorio correcto
if [ ! -f "NuevoForo.slnx" ]; then
	echo -e "${RED}❌ Error: No se encontró NuevoForo.slnx${NC}"
	echo "Ejecuta este script desde la raíz del proyecto"
	exit 1
fi

# Paso 1: Verificar Git
echo -e "\n${BLUE}Paso 1: Verificando Git...${NC}"
if ! git status > /dev/null 2>&1; then
	echo -e "${RED}❌ Git no está inicializado${NC}"
	exit 1
fi
echo -e "${GREEN}✅ Git inicializado${NC}"

# Paso 2: Verificar cambios sin commitear
echo -e "\n${BLUE}Paso 2: Verificando cambios pendientes...${NC}"
CHANGES=$(git status --short)
if [ -n "$CHANGES" ]; then
	echo -e "${BLUE}Cambios detectados:${NC}"
	echo "$CHANGES"
	echo -e "\n${BLUE}¿Deseas commitear estos cambios? (s/n)${NC}"
	read -r RESPONSE
	if [ "$RESPONSE" = "s" ] || [ "$RESPONSE" = "S" ]; then
		git add .
		echo -e "${BLUE}Mensaje del commit (default: 'Update'):${NC}"
		read -r COMMIT_MSG
		COMMIT_MSG=${COMMIT_MSG:-"Update"}
		git commit -m "$COMMIT_MSG"
		echo -e "${GREEN}✅ Cambios commiteados${NC}"
	fi
fi

# Paso 3: Verificar rama main
echo -e "\n${BLUE}Paso 3: Verificando rama...${NC}"
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
echo -e "Rama actual: ${BLUE}$CURRENT_BRANCH${NC}"

if [ "$CURRENT_BRANCH" != "main" ]; then
	echo -e "${BLUE}¿Deseas cambiar a rama 'main'? (s/n)${NC}"
	read -r RESPONSE
	if [ "$RESPONSE" = "s" ] || [ "$RESPONSE" = "S" ]; then
		git branch -M main
		echo -e "${GREEN}✅ Rama renombrada a 'main'${NC}"
	fi
fi

# Paso 4: Compilar el proyecto
echo -e "\n${BLUE}Paso 4: Compilando proyecto...${NC}"
dotnet build -c Release
if [ $? -ne 0 ]; then
	echo -e "${RED}❌ Error en compilación${NC}"
	exit 1
fi
echo -e "${GREEN}✅ Compilación exitosa${NC}"

# Paso 5: Push a GitHub
echo -e "\n${BLUE}Paso 5: Pusheando cambios a GitHub...${NC}"
echo -e "${BLUE}Nota: Configura tu remote GitHub primero${NC}"
echo -e "${BLUE}Ejemplo: git remote add origin https://github.com/TU_USUARIO/NuevoForo.git${NC}"

REMOTE=$(git remote get-url origin 2>/dev/null)
if [ -z "$REMOTE" ]; then
	echo -e "${RED}❌ No hay remote configurado${NC}"
	echo -e "${BLUE}Agrega GitHub como remote:${NC}"
	echo "git remote add origin <URL_DE_TU_REPO_GITHUB>"
	exit 1
fi

echo -e "Remote GitHub: ${BLUE}$REMOTE${NC}"
echo -e "${BLUE}¿Deseas pushear a GitHub? (s/n)${NC}"
read -r RESPONSE
if [ "$RESPONSE" = "s" ] || [ "$RESPONSE" = "S" ]; then
	git push -u origin main
	if [ $? -ne 0 ]; then
		echo -e "${RED}❌ Error al pushear${NC}"
		exit 1
	fi
	echo -e "${GREEN}✅ Push exitoso${NC}"
fi

# Paso 6: Verificar CLI de Railway
echo -e "\n${BLUE}Paso 6: Verificando Railway CLI...${NC}"
if ! command -v railway &> /dev/null; then
	echo -e "${RED}❌ Railway CLI no está instalado${NC}"
	echo -e "${BLUE}Instálalo con: npm install -g @railway/cli${NC}"
else
	echo -e "${GREEN}✅ Railway CLI instalado${NC}"

	# Paso 7: Login en Railway
	echo -e "\n${BLUE}Paso 7: Verificando autenticación en Railway...${NC}"
	if railway status > /dev/null 2>&1; then
		echo -e "${GREEN}✅ Autenticado en Railway${NC}"
	else
		echo -e "${BLUE}Necesitas autenticarte en Railway${NC}"
		railway login
	fi

	# Paso 8: Desplegar
	echo -e "\n${BLUE}Paso 8: Desplegando en Railway...${NC}"
	echo -e "${BLUE}¿Deseas desplegar ahora? (s/n)${NC}"
	read -r RESPONSE
	if [ "$RESPONSE" = "s" ] || [ "$RESPONSE" = "S" ]; then
		railway up
		if [ $? -eq 0 ]; then
			echo -e "${GREEN}✅ Despliegue enviado a Railway${NC}"
			echo -e "\n${BLUE}Próximos pasos:${NC}"
			echo "1. Ve a https://railway.app/dashboard"
			echo "2. Verifica los logs del despliegue"
			echo "3. Espera a que las migraciones se completen"
			echo "4. Accede a tu aplicación en la URL proporcionada"
		fi
	fi
fi

echo -e "\n${BLUE}========================================${NC}"
echo -e "${GREEN}✅ Script completado${NC}"
echo -e "${BLUE}========================================${NC}"
