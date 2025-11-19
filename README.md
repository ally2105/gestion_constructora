# Sistema de Gestión de Construcción - Firmeza

Este proyecto es una aplicación full-stack desarrollada con ASP.NET Core que implementa un sistema de gestión para una empresa constructora. La aplicación permite administrar productos, clientes, ventas y usuarios a través de un panel de administración web y una API RESTful.

## Características Principales

- **Panel de Administración:** Interfaz web (ASP.NET Core MVC) para la gestión de entidades de negocio.
- **API RESTful:** API (ASP.NET Core Web API) que expone la lógica de negocio para ser consumida por clientes externos (ej. una futura app Blazor o móvil).
- **Gestión de Productos:** CRUD completo para productos, con validaciones y búsqueda.
- **Gestión de Clientes:** CRUD completo para clientes, integrado con el sistema de usuarios de Identity.
- **Gestión de Ventas:** Creación de ventas, actualización de stock y generación de recibos.
- **Autenticación y Autorización:** Sistema de roles (Administrador, Cliente) basado en ASP.NET Core Identity. La API utiliza autenticación JWT.
- **Importación Masiva:** Carga de datos de ventas desnormalizados desde archivos Excel (.xlsx) usando EPPlus.
- **Generación de PDF:** Creación de recibos de venta en formato PDF.

## Arquitectura del Proyecto

El sistema sigue una **Arquitectura por Capas** (también conocida como Arquitectura Limpia o N-Capas) para asegurar la separación de responsabilidades, mantenibilidad y testabilidad.

```
+--------------------------+      +-------------------------+
| gestion_construccion.web |      | gestion_construcion.api |  <-- Capa de Presentación
+--------------------------+      +-------------------------+
             |                                |
             +----------------+---------------+
                              |
                              v
+-----------------------------------------------------------+
|                  Firmeza.Infrastructure                   |  <-- Capa de Infraestructura
| (Implementaciones: Repositorios, Servicios, etc.)         |
+-----------------------------------------------------------+
                              |
                              v
+-----------------------------------------------------------+
|                       Firmeza.Core                        |  <-- Núcleo / Dominio
| (Entidades, Interfaces, Lógica de Negocio Pura)           |
+-----------------------------------------------------------+
```

1.  **`Firmeza.Core` (Capa de Dominio):**
    - Es el corazón de la aplicación. Contiene las entidades de negocio (`Cliente`, `Producto`), las interfaces (`IClienteService`, `IUnitOfWork`), DTOs compartidos y la lógica de negocio pura. No depende de ninguna otra capa.

2.  **`Firmeza.Infrastructure` (Capa de Infraestructura):**
    - Implementa las interfaces definidas en `Firmeza.Core`. Contiene el código que interactúa con sistemas externos, como la base de datos (repositorios, `UnitOfWork`) y servicios de terceros (envío de emails con `SmtpEmailService`).

3.  **Capas de Presentación (UI & API):**
    - **`gestion_construccion.web`:** Proyecto ASP.NET Core MVC con Razor. Contiene los controladores, vistas y ViewModels para el panel de administración.
    - **`gestion_construcion.api`:** Proyecto ASP.NET Core Web API. Expone la lógica de negocio a través de endpoints RESTful, utilizando DTOs para los contratos de la API y autenticación JWT.

4.  **`Firmeza.Tests` (Capa de Pruebas):**
    - Proyecto de pruebas unitarias que utiliza `xUnit` y `Moq` para verificar la funcionalidad de las otras capas.

## Tecnologías Utilizadas

- **Backend:** C#, ASP.NET Core 8, Entity Framework Core 8
- **Frontend:** HTML, CSS, Bootstrap 5
- **Base de Datos:** PostgreSQL
- **Autenticación:** ASP.NET Core Identity (para la web), JWT (para la API)
- **Librerías Clave:**
  - `AutoMapper`: Para mapear entre entidades y DTOs/ViewModels.
  - `EPPlus`: Para la lectura de archivos Excel.
  - `iTextSharp`: Para la generación de archivos PDF (se recomienda migrar a una alternativa moderna).
  - `MailKit`: Para el envío de correos electrónicos.
  - `Npgsql.EntityFrameworkCore.PostgreSQL`: Proveedor de EF Core para PostgreSQL.
  - `Swashbuckle`: Para la documentación de la API (Swagger).

## Instrucciones de Instalación y Ejecución

### Ejecución Local (requiere .NET 8 SDK y PostgreSQL)

1.  **Clonar el Repositorio:**
    ```bash
    git clone <URL_DEL_REPOSITORIO>
    cd Firmeza
    ```

2.  **Configurar la Base de Datos:**
    - Asegúrate de tener una instancia de PostgreSQL en ejecución.
    - Abre los archivos `gestion_construccion.web/appsettings.json` y `gestion_construcion.api/appsettings.json`.
    - Modifica la `ConnectionString` en ambos archivos con tus credenciales de PostgreSQL:
      ```json
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=gestion_construccion;Username=TU_USUARIO;Password=TU_CONTRASEÑA"
      }
      ```

3.  **Aplicar Migraciones:**
    - **Importante:** Las migraciones ahora se gestionan desde el proyecto `gestion_construccion.web`.
    - Navega a la carpeta del proyecto web:
      ```bash
      cd gestion_construccion.web
      ```
    - Ejecuta el siguiente comando para crear la base de datos y aplicar el esquema:
      ```bash
      dotnet ef database update
      ```

4.  **Ejecutar la Aplicación (Web y API):**
    - Desde la carpeta raíz de la solución (`Firmeza`), ejecuta el siguiente comando:
      ```bash
      dotnet run --project gestion_construccion.web
      ```
    - En otra terminal, ejecuta la API:
      ```bash
      dotnet run --project gestion_construcion.api
      ```
    - La aplicación web estará disponible en la URL que indique la consola (ej: `http://localhost:5031`).
    - La API estará disponible en otra URL (ej: `http://localhost:5001`).

5.  **Acceder a la Aplicación:**
    - Navega a la URL de la aplicación web.
    - Inicia sesión con las credenciales de administrador por defecto:
      - **Email:** `admin@firmeza.com`
      - **Password:** `Admin123!`
    - Puedes explorar la API a través de su URL de Swagger (ej: `http://localhost:5001/swagger`).

### Ejecución con Docker (Recomendado)

El proyecto está configurado para ejecutarse fácilmente con Docker y Docker Compose.

1.  **Requisitos:** Tener Docker y Docker Compose instalados.

2.  **Ejecutar Docker Compose:**
    - Desde la carpeta raíz de la solución (`Firmeza`), ejecuta el siguiente comando:
      ```bash
      docker-compose up --build
      ```
    - Este comando hará lo siguiente:
      - Construirá imágenes para la aplicación web y la API.
      - Creará un contenedor para la base de datos PostgreSQL.
      - Iniciará todos los servicios y los conectará.

3.  **Acceder a la Aplicación:**
    - La aplicación web estará disponible en `http://localhost:8080`.
    - La API estará disponible en `http://localhost:8081`.
    - La base de datos PostgreSQL estará accesible en el puerto `5432` de tu máquina local.
    - El usuario administrador se creará automáticamente al iniciar la aplicación.
```