# Fase 1: Compilación
# Usamos la imagen del SDK de .NET 8 para compilar la aplicación.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar los archivos de proyecto (.csproj) y restaurar las dependencias.
# Se copian por separado para aprovechar la caché de capas de Docker.
COPY ["gestion_construccion.web/gestion_construccion.web.csproj", "gestion_construccion.web/"]
COPY ["gestion_construcion.api/gestion_construcion.api.csproj", "gestion_construcion.api/"]
COPY ["Firmeza.sln", "."]
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
# Usamos la imagen de ASP.NET, que es más ligera porque no incluye el SDK.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Exponer el puerto 8080 para que la aplicación sea accesible.
EXPOSE 8080
# El comando para iniciar la aplicación cuando el contenedor se ejecute.
ENTRYPOINT ["dotnet", "gestion_construccion.web.dll"]
```