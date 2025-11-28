FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["gestion_construccion.web/gestion_construccion.web.csproj", "gestion_construccion.web/"]
COPY ["Firmeza.Core/Firmeza.Core.csproj", "Firmeza.Core/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]
COPY ["gestion_construcion.api/gestion_construcion.api.csproj", "gestion_construcion.api/"]
RUN dotnet restore "gestion_construccion.web/gestion_construccion.web.csproj"
COPY . .
WORKDIR "/src/gestion_construccion.web"
RUN dotnet build "gestion_construccion.web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gestion_construccion.web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gestion_construccion.web.dll"]
