# Proyecto Firmeza: Sistema de Gestión de Construcción

## 🏗️ Descripción General

**Firmeza** es un sistema integral para la gestión de productos y ventas en el sector de la construcción. Está desarrollado con una arquitectura de microservicios que incluye una API REST, un frontend para clientes (SPA) y un panel de administración.

El sistema permite a los clientes registrarse, iniciar sesión, explorar un catálogo de productos, gestionar un carrito de compras, realizar pedidos y recibir confirmaciones por correo electrónico con el comprobante de venta en formato PDF. El panel de administración ofrece funcionalidades para gestionar productos, clientes y ventas, incluyendo la importación masiva de datos desde archivos Excel.

## ✨ Características Principales

### Frontend de Clientes (React SPA)
- **Autenticación JWT:** Registro e inicio de sesión seguro.
- **Catálogo de Productos:** Visualización paginada de productos con diseño corporativo.
- **Carrito de Compras:** Gestión de productos seleccionados, cálculo de totales (subtotal, IVA, total).
- **Proceso de Compra:** Envío de pedidos a la API.
- **Notificaciones:** Mensajes de éxito/error mediante "toast" no intrusivos.
- **Diseño UI/UX:** Interfaz moderna, limpia y coherente con la paleta de colores corporativa.

### Backend (ASP.NET Core API REST)
- **Autenticación y Autorización:** Basada en JWT y roles (Cliente, Administrador).
- **Gestión de Usuarios:** Integración con ASP.NET Core Identity.
- **Gestión de Productos:** CRUD completo.
- **Gestión de Clientes:** CRUD completo.
- **Gestión de Ventas:** Creación de ventas y detalles asociados.
- **Generación de PDF:** Creación de comprobantes de venta en formato PDF.
- **Servicio de Correo:** Envío de correos electrónicos (confirmaciones, comprobantes) utilizando SMTP real (Gmail).

### Panel de Administración (ASP.NET Core MVC)
- **Autenticación:** Inicio de sesión para administradores.
- **Gestión de Productos:** CRUD de productos.
- **Gestión de Clientes:** CRUD de clientes.
- **Gestión de Ventas:** Visualización de ventas.
- **Importación de Datos:** Importación masiva de datos desde archivos Excel.
- **Diseño UI/UX:** Interfaz profesional y coherente con la paleta de colores corporativa.

## 🚀 Tecnologías Utilizadas

### Frontend
- **React:** Librería para construir interfaces de usuario.
- **Vite:** Herramienta de construcción rápida para proyectos frontend.
- **Axios:** Cliente HTTP para comunicación con la API.
- **React Router DOM:** Para la navegación en la SPA.
- **jwt-decode:** Para decodificar tokens JWT en el cliente.
- **react-hot-toast:** Para notificaciones "toast".
- **CSS Personalizado:** Para el diseño UI/UX.

### Backend
- **.NET 8:** Framework para construir la API REST y el panel de administración.
- **Entity Framework Core:** ORM para interacción con la base de datos.
- **PostgreSQL:** Base de datos relacional.
- **ASP.NET Core Identity:** Sistema de gestión de usuarios y roles.
- **JWT (JSON Web Tokens):** Para autenticación segura.
- **AutoMapper:** Para mapeo de objetos entre DTOs y entidades.
- **MailKit:** Librería para el envío de correos electrónicos vía SMTP.
- **iTextSharp:** Librería para la generación de documentos PDF.
- **xUnit:** Framework para pruebas unitarias.
- **Moq:** Librería para simulación de objetos en pruebas.

### DevOps
- **Docker:** Para la contenedorización de la aplicación.
- **Docker Compose:** Para la orquestación de los servicios.

## 🐳 Ejecución con Docker (Recomendado)

La forma más sencilla de levantar todo el entorno (base de datos, API, frontend, admin) es a través de Docker Compose.

### Prerrequisitos
- **Docker:** [Instalar Docker](https://docs.docker.com/get-docker/)
- **Docker Compose:** (Viene incluido con Docker Desktop)

### Pasos
1. **Clona el repositorio:**
   ```bash
   git clone <URL_DEL_REPOSITORIO>
   cd Firmeza
   ```

2. **Levanta todos los servicios:**
   - Abre una terminal en la raíz del proyecto (`/Firmeza/`).
   - Ejecuta el siguiente comando:
     ```bash
     docker compose up --build
     ```
   - Este comando realizará las siguientes acciones:
     1. Construirá las imágenes de Docker para cada servicio.
     2. Ejecutará las pruebas unitarias.
     3. Si las pruebas pasan, levantará los contenedores para la base de datos, la API, el panel de administración y el frontend de clientes.

3. **Accede a los servicios:**
   - **Frontend de Clientes:** `http://localhost:3000`
   - **Panel de Administración:** `http://localhost:5031`
   - **API (Swagger):** `http://localhost:5165/swagger`

## 🛠️ Ejecución en Local (Sin Docker)

Si prefieres ejecutar los servicios localmente sin Docker, sigue estos pasos.

### Prerrequisitos
- **.NET SDK 8.0:** [Descargar .NET](https://dotnet.microsoft.com/download)
- **Node.js y npm:** [Descargar Node.js](https://nodejs.org/)
- **PostgreSQL:** Servidor de base de datos.

### Configuración
1. **Base de Datos:**
   - Asegúrate de que tu servidor PostgreSQL esté en ejecución.
   - En los archivos `appsettings.json` de `gestion_construcion.api` y `gestion_construccion.web`, verifica que la cadena de conexión `DefaultConnection` apunte a tu instancia de PostgreSQL.

2. **Servicio de Correo (Gmail SMTP):**
   - En los archivos `appsettings.json` de `gestion_construcion.api` y `gestion_construccion.web`, actualiza la sección `SmtpSettings` con tus credenciales de Gmail y una "Contraseña de Aplicación".

### Ejecución
1. **Iniciar la API:**
   ```bash
   cd gestion_construcion.api
   dotnet run
   ```

2. **Iniciar el Panel de Administración:**
   ```bash
   cd gestion_construccion.web
   dotnet run
   ```

3. **Iniciar el Frontend de Clientes:**
   ```bash
   cd Firmeza.Client
   npm install
   npm run dev
   ```

## 🔑 Credenciales por Defecto

### Panel de Administración
- **Usuario:** `admin@firmeza.com`
- **Contraseña:** `Admin123!`

### Frontend de Clientes
- Puedes registrar nuevos usuarios desde la página de registro.

## 📂 Estructura del Proyecto

```
Firmeza/
├── Firmeza.Core/                  # Contratos, Modelos de Dominio, DTOs
├── Firmeza.Infrastructure/        # Repositorios, Servicios (EF Core, Email, PDF)
├── Firmeza.Client/                # Frontend de Clientes (React SPA)
├── gestion_construcion.api/       # Backend (ASP.NET Core API REST)
├── gestion_construccion.web/      # Panel de Administración (ASP.NET Core MVC)
├── Firmeza.Tests/                 # Pruebas Unitarias (xUnit)
├── docker-compose.yml             # Orquestación de contenedores
└── README.md                      # Este archivo
```
