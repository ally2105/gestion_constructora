FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar archivos de solución y proyectos para restaurar dependencias
# Solo copiar los proyectos necesarios para Firmeza.Tests
COPY ["Firmeza.Tests/Firmeza.Tests.csproj", "Firmeza.Tests/"]
COPY ["Firmeza.Core/Firmeza.Core.csproj", "Firmeza.Core/"]
COPY ["Firmeza.Infrastructure/Firmeza.Infrastructure.csproj", "Firmeza.Infrastructure/"]

# Restaurar dependencias para la solución completa (o al menos para los proyectos referenciados)
# Es mejor restaurar a nivel de solución si hay muchas referencias cruzadas
# Para simplificar, restauramos solo el proyecto de pruebas y sus dependencias
RUN dotnet restore "Firmeza.Tests/Firmeza.Tests.csproj"

# Copiar el resto de los archivos del proyecto de pruebas
COPY . .

# Compilar el proyecto de pruebas (sin publicar)
WORKDIR /src/Firmeza.Tests
RUN dotnet build "Firmeza.Tests.csproj" -c Debug

# Ejecutar las pruebas
# Usamos el .csproj como argumento para dotnet test
ENTRYPOINT ["dotnet", "test", "Firmeza.Tests.csproj", "--no-build", "--verbosity", "normal"]
