FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar archivos de proyecto para aprovechar la caché
COPY ["Firmeza.Tests/Firmeza.Tests.csproj", "Firmeza.Tests/"]
COPY ["Firmeza.Core/Firmeza.Core.csproj", "Firmeza.Core/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]
COPY ["gestion_construccion.web/gestion_construccion.web.csproj", "gestion_construccion.web/"]

# Restaurar dependencias
RUN dotnet restore "Firmeza.Tests/Firmeza.Tests.csproj"

# Copiar el resto del código
COPY . .

# Compilar
WORKDIR /src/Firmeza.Tests
RUN dotnet build "Firmeza.Tests.csproj" -c Debug --no-restore

# El entrypoint ejecuta las pruebas y sale con el código de estado correspondiente
ENTRYPOINT ["dotnet", "test", "Firmeza.Tests.csproj", "--no-build", "--verbosity", "normal"]
