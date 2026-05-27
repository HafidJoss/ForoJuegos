# Dockerfile optimizado para .NET 10 con migraciones automáticas

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copiar archivos de proyecto
COPY ["src/NuevoForo.Api/NuevoForo.Api.csproj", "src/NuevoForo.Api/"]
COPY ["src/NuevoForo.Application/NuevoForo.Application.csproj", "src/NuevoForo.Application/"]
COPY ["src/NuevoForo.Domain/NuevoForo.Domain.csproj", "src/NuevoForo.Domain/"]
COPY ["src/NuevoForo.Infrastructure/NuevoForo.Infrastructure.csproj", "src/NuevoForo.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "src/NuevoForo.Api/NuevoForo.Api.csproj"

# Copiar código fuente
COPY . .

# Construir aplicación
RUN dotnet build "src/NuevoForo.Api/NuevoForo.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish

RUN dotnet publish "src/NuevoForo.Api/NuevoForo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app

# Instalar herramientas necesarias
RUN apt-get update && apt-get install -y --no-install-recommends \
	curl \
	&& rm -rf /var/lib/apt/lists/*

# Copiar binarios publicados
COPY --from=publish /app/publish .

# Crear script de inicio que ejecuta migraciones
RUN echo '#!/bin/bash\n\
set -e\n\
echo "Ejecutando migraciones de base de datos..."\n\
dotnet NuevoForo.Api.dll migrate\n\
\n\
echo "Iniciando aplicación..."\n\
exec dotnet NuevoForo.Api.dll' > /app/entrypoint.sh && \
	chmod +x /app/entrypoint.sh

# Exponer puerto (Railway inyecta PORT en runtime)
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
	CMD curl -f http://localhost:8080/health || exit 1

# Usar el script de entrada
ENTRYPOINT ["/app/entrypoint.sh"]
