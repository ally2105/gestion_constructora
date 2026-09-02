FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Railway inyecta dinámicamente la variable PORT
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar proyectos necesarios para la API (.csproj)
COPY ["gestion_construcion.api/gestion_construcion.api.csproj", "gestion_construcion.api/"]
COPY ["Firmeza.Core/Firmeza.Core.csproj", "Firmeza.Core/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]

# Restaurar dependencias exclusivamente para la API
RUN dotnet restore "gestion_construcion.api/gestion_construcion.api.csproj"

# Copiar todo el código fuente
COPY . .

WORKDIR "/src/gestion_construcion.api"
RUN dotnet build "gestion_construcion.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gestion_construcion.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gestion_construcion.api.dll"]
