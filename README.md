# Sistema de Gestión de Construcción - Firmeza

Este proyecto es una aplicación web full-stack desarrollada con ASP.NET Core que implementa un sistema de gestión para una empresa constructora. La aplicación permite administrar productos, clientes, ventas y usuarios a través de un panel de administración seguro y moderno.

## Características Principales

- **Panel de Administración:** Interfaz web para la gestión de entidades de negocio.
- **Gestión de Productos:** CRUD completo para productos, con validaciones y búsqueda.
- **Gestión de Clientes:** CRUD completo para clientes, integrado con el sistema de usuarios de Identity.
- **Gestión de Ventas:** Creación de ventas, actualización de stock y generación de recibos.
- **Autenticación y Autorización:** Sistema de roles (Administrador, Cliente) basado en ASP.NET Core Identity.
- **Importación Masiva:** Carga de datos de ventas desnormalizados desde archivos Excel (.xlsx) usando EPPlus.
- **Generación de PDF:** Creación de recibos de venta en formato PDF con iTextSharp.
- **API REST:** Proyecto de API separado para exponer la lógica de negocio (ej: `api/Productos`).

## Arquitectura del Proyecto

El sistema sigue una arquitectura por capas para asegurar la separación de responsabilidades, mantenibilidad y testabilidad.

1.  **Capa de Presentación (UI):**
    -   **`gestion_construccion.web`:** Un proyecto ASP.NET Core MVC con Razor Views. Contiene los controladores, vistas y ViewModels para la interfaz de usuario del panel de administración.
    -   **`Firmeza.Api`:** Un proyecto ASP.NET Core Web API que expone la lógica de negocio a través de endpoints RESTful.

2.  **Capa de Lógica de Negocio (Services):**
    -   Contiene la lógica de negocio principal. Los servicios orquestan las operaciones, validan datos y llaman a la capa de acceso a datos.
    -   Ejemplos: `ClienteService`, `ProductoService`, `VentaService`, `ImportService`.

3.  **Capa de Acceso a Datos (Repositories):**
    -   Implementa el **Patrón Repositorio** y el **Patrón Unidad de Trabajo (Unit of Work)** para abstraer la interacción con la base de datos.
    -   `IRepository<T>` y `Repository<T>`: Repositorio genérico para operaciones CRUD básicas.
    -   `IUnitOfWork` y `UnitOfWork`: Coordina los repositorios y asegura la atomicidad de las transacciones.

4.  **Capa de Dominio (Models):**
    -   Contiene las entidades principales del negocio (`Producto`, `Cliente`, `Venta`) y las clases de Identity (`Usuario`, `Rol`).

5.  **Capa de Infraestructura (Datos):**
    -   `AppDbContext`: El contexto de Entity Framework Core que gestiona la conexión con la base deatos y el mapeo de las entidades.
    -   Configurado para usar **PostgreSQL** como motor de base de datos.

## Tecnologías Utilizadas

- **Backend:** C#, ASP.NET Core 8, Entity Framework Core 8
- **Frontend:** HTML, CSS, Bootstrap 5
- **Base de Datos:** PostgreSQL
- **Autenticación:** ASP.NET Core Identity
- **Librerías Clave:**
  - `EPPlus`: Para la lectura de archivos Excel.
  - `iTextSharp`: Para la generación de archivos PDF.
  - `Npgsql.EntityFrameworkCore.PostgreSQL`: Proveedor de EF Core para PostgreSQL.

## Instrucciones de Instalación y Ejecución

### Ejecución Local (requiere .NET 8 SDK y PostgreSQL)

1.  **Clonar el Repositorio:**
    ```bash
    git clone <URL_DEL_REPOSITORIO>
    cd Firmeza
    ```

2.  **Configurar la Base de Datos:**
    - Asegúrate de tener una instancia de PostgreSQL en ejecución.
    - Abre el archivo `gestion_construccion.web/appsettings.json`.
    - Modifica la `ConnectionString` con tus credenciales de PostgreSQL:
      ```json
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=gestion_construccion;Username=TU_USUARIO;Password=TU_CONTRASEÑA"
      }
      ```

3.  **Aplicar Migraciones:**
    - Navega a la carpeta del proyecto web:
      ```bash
      cd gestion_construccion.web
      ```
    - Ejecuta los siguientes comandos para crear la base de datos y aplicar el esquema:
      ```bash
      dotnet ef database update
      ```

4.  **Ejecutar la Aplicación Web:**
    ```bash
    dotnet run
    ```
    La aplicación estará disponible en la URL que indique la consola (ej: `http://localhost:5031`).

5.  **Acceder a la Aplicación:**
    - Navega a la URL de la aplicación.
    - Inicia sesión con las credenciales de administrador por defecto:
      - **Email:** `admin@firmeza.com`
      - **Password:** `Admin123!`

### Ejecución con Docker (Recomendado)

El proyecto está configurado para ejecutarse fácilmente con Docker y Docker Compose.

1.  **Requisitos:** Tener Docker y Docker Compose instalados.

2.  **Ejecutar Docker Compose:**
    - Desde la carpeta raíz de la solución (`Firmeza`), ejecuta el siguiente comando:
      ```bash
      docker-compose up --build
      ```
    - Este comando hará lo siguiente:
      - Construirá una imagen para la aplicación web.
      - Creará un contenedor para la base de datos PostgreSQL.
      - Iniciará ambos servicios y los conectará.

3.  **Acceder a la Aplicación:**
    - La aplicación web estará disponible en `http://localhost:8080` (o el puerto que hayas configurado en `docker-compose.yml`).
    - La base de datos PostgreSQL estará accesible en el puerto `5432` de tu máquina local.
    - El usuario administrador se creará automáticamente al iniciar la aplicación.
```