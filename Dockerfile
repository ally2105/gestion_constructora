# Fase 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar los archivos de proyecto (.csproj) y restaurar las dependencias.
# Se copian por separado para aprovechar la caché de capas de Docker.
COPY ["gestion_construccion.web/gestion_construccion.web.csproj", "gestion_construccion.web/"]
COPY ["gestion_construcion.api/gestion_construcion.api.csproj", "gestion_construcion.api/"]

# 🔥 AGREGADO: proyectos que Firmeza.sln está pidiendo

COPY ["Firmeza.Tests/Firmeza.Tests.csproj", "Firmeza.Tests/"]
COPY ["Firmeza.Core/Firmeza.Core.csproj", "Firmeza.Core/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]

# Copiar la solución
COPY ["Firmeza.sln", "."]

# Restaurar dependencias
RUN dotnet restore "Firmeza.sln"

# Copiar el resto del código fuente.
COPY . .
WORKDIR "/src/gestion_construccion.web"

# Compilar la aplicación en modo Release.
RUN dotnet build "gestion_construccion.web.csproj" -c Release -o /app/build

# Fase 2: Publicación
FROM build AS publish
RUN dotnet publish "gestion_construccion.web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Fase 3: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "gestion_construccion.web.dll"]
