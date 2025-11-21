# Proyecto Firmeza: Gestión de Construcción

## Descripción General

Este proyecto es un sistema integral para la gestión de productos y ventas en el sector de la construcción, desarrollado con una arquitectura de microservicios (API REST) y dos interfaces de usuario: un frontend de clientes (SPA con React) y un panel de administración (ASP.NET Core MVC).

El sistema permite a los clientes registrarse, iniciar sesión, explorar un catálogo de productos, añadir productos a un carrito de compras, realizar pedidos y recibir confirmaciones por correo electrónico con el comprobante de venta en formato PDF. El panel de administración ofrece funcionalidades para gestionar productos, clientes y ventas.

## Características Principales

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
- **Diseño UI/UX:** Interfaz profesional y coherente con la paleta de colores corporativa.

## Tecnologías Utilizadas

### Frontend
- **React:** Librería para construir interfaces de usuario.
- **Vite:** Herramienta de construcción rápida para proyectos frontend.
- **Axios:** Cliente HTTP para comunicación con la API.
- **React Router DOM:** Para la navegación en la SPA.
- **jwt-decode:** Para decodificar tokens JWT en el cliente.
- **react-hot-toast:** Para notificaciones "toast".
- **CSS Personalizado:** Para el diseño UI/UX.

### Backend
- **ASP.NET Core 8:** Framework para construir la API REST y el panel de administración.
- **Entity Framework Core:** ORM para interacción con la base de datos.
- **PostgreSQL:** Base de datos relacional.
- **ASP.NET Core Identity:** Sistema de gestión de usuarios y roles.
- **JWT (JSON Web Tokens):** Para autenticación segura.
- **AutoMapper:** Para mapeo de objetos entre DTOs y entidades.
- **MailKit:** Librería para el envío de correos electrónicos vía SMTP.
- **iTextSharp:** Librería para la generación de documentos PDF.
- **Microsoft.Extensions.Logging:** Para el registro de eventos.

## Configuración del Entorno

### Prerrequisitos
Asegúrate de tener instalado lo siguiente:
- **.NET SDK 8.0:** [Descargar .NET](https://dotnet.microsoft.com/download)
- **Node.js y npm:** [Descargar Node.js](https://nodejs.org/)
- **PostgreSQL:** Servidor de base de datos. Puedes instalarlo localmente o usar Docker.
- **Git:** Para clonar el repositorio.

### Configuración de la Base de Datos (PostgreSQL)
1. Asegúrate de que tu servidor PostgreSQL esté en ejecución.
2. **Cadena de Conexión:** Abre los archivos `appsettings.json` en `gestion_construcion.api` y `gestion_construccion.web`.
   - `Firmeza/gestion_construcion.api/appsettings.json`
   - `Firmeza/gestion_construccion.web/appsettings.json`
   - Verifica que la `DefaultConnection` apunte a tu instancia de PostgreSQL. Ejemplo:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=gestion_construccion;Username=postgres;Password=Qwe.123*"
     }
     ```
     (Asegúrate de que `Database`, `Username` y `Password` coincidan con tu configuración de PostgreSQL).

3. **Migraciones de Entity Framework Core:**
   - Abre una terminal en la carpeta `Firmeza/gestion_construcion.api`.
   - Ejecuta los siguientes comandos para aplicar las migraciones y crear la base de datos:
     ```bash
     dotnet ef database update
     ```
     (Si es la primera vez, es posible que necesites `dotnet ef migrations add InitialCreate` primero, pero el proyecto ya debería tenerlas).

### Configuración del Servicio de Correo (Gmail SMTP)
1. **Generar Contraseña de Aplicación de Gmail:**
   - Ve a [myaccount.google.com](https://myaccount.google.com).
   - En "Seguridad", asegúrate de que la "Verificación en dos pasos" esté **ACTIVADA**.
   - Busca "Contraseñas de Aplicación" y genera una nueva para "Correo" y "Otro (nombre personalizado)". Copia la contraseña de 16 caracteres.
2. **Actualizar `appsettings.json`:**
   - En `Firmeza/gestion_construcion.api/appsettings.json` y `Firmeza/gestion_construccion.web/appsettings.json`, actualiza la sección `SmtpSettings` con tus credenciales de Gmail y la contraseña de aplicación generada:
     ```json
     "SmtpSettings": {
       "Server": "smtp.gmail.com",
       "Port": 587,
       "SenderName": "Firmeza Construcción",
       "SenderEmail": "tu_email_de_gmail@gmail.com", // Tu dirección de Gmail
       "Username": "tu_email_de_gmail@gmail.com",     // Tu dirección de Gmail
       "Password": "TU_CONTRASEÑA_DE_APLICACION"       // La contraseña de 16 caracteres
     }
     ```
     (Reemplaza `tu_email_de_gmail@gmail.com` y `TU_CONTRASEÑA_DE_APLICACION` con tus datos reales).

### Configuración JWT
- La configuración JWT se encuentra en `Firmeza/gestion_construcion.api/appsettings.json`. Asegúrate de que `Key` sea una cadena larga y segura.
  ```json
  "Jwt": {
    "Key": "ESTA_ES_MI_CLAVE_SECRETA_SUPER_LARGA_Y_COMPLEJA_PARA_FIRMAR_TOKENS_JWT_DEBE_TENER_AL_MENOS_32_CARACTERES",
    "Issuer": "FirmezaAPI",
    "Audience": "FirmezaClientes"
  }
  ```

## Ejecución del Proyecto

El proyecto consta de tres partes que se ejecutan de forma independiente: la API, el Frontend de Clientes y el Panel de Administración.

### 1. Iniciar la API (Backend)
- Abre una terminal en la carpeta `Firmeza/gestion_construcion.api`.
- Ejecuta:
  ```bash
  dotnet run
  ```
- La API se iniciará y estará disponible en `http://localhost:5165`.

### 2. Iniciar el Frontend de Clientes (React SPA)
- Abre **otra** terminal en la carpeta `Firmeza/Firmeza.Client`.
- Primero, instala las dependencias (solo la primera vez o si `package.json` cambia):
  ```bash
  npm install
  ```
- Luego, inicia la aplicación:
  ```bash
  npm run dev
  ```
- El frontend se iniciará y estará disponible en `http://localhost:3000`.

### 3. Iniciar el Panel de Administración (ASP.NET Core MVC)
- Abre **otra** terminal en la carpeta `Firmeza/gestion_construccion.web`.
- Ejecuta:
  ```bash
  dotnet run
  ```
- El panel de administración se iniciará y estará disponible en `http://localhost:5031` (o el puerto configurado).

## Credenciales por Defecto

### Panel de Administración
- **Usuario:** `admin@firmeza.com`
- **Contraseña:** `Admin123!`
  (Este usuario se crea automáticamente si la base de datos está vacía y se ejecuta `SeedData`).

### Frontend de Clientes
- Puedes registrar nuevos usuarios desde la página de registro (`http://localhost:3000/register`).

## Estructura del Proyecto

```
Firmeza/
├── Firmeza.Core/                  # Contratos (Interfaces), Modelos de Dominio, DTOs
├── Firmeza.Infrastructure/        # Implementaciones de Interfaces, Repositorios, Servicios (EF Core, Email, PDF)
├── Firmeza.Client/                # Frontend de Clientes (React SPA)
│   ├── public/
│   ├── src/
│   │   ├── components/
│   │   ├── context/
│   │   ├── pages/
│   │   ├── services/
│   │   ├── styles/
│   │   ├── App.jsx
│   │   └── main.jsx
│   ├── index.html
│   ├── package.json
│   └── vite.config.js
├── gestion_construcion.api/       # Backend (ASP.NET Core API REST)
│   ├── Controllers/
│   ├── DTOs/
│   ├── Profiles/
│   ├── appsettings.json
│   ├── Program.cs
│   └── gestion_construcion.api.csproj
├── gestion_construccion.web/      # Panel de Administración (ASP.NET Core MVC)
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   ├── wwwroot/
│   │   ├── css/admin-theme.css
│   │   └── ...
│   ├── appsettings.json
│   ├── Program.cs
│   └── gestion_construccion.web.csproj
├── Firmeza.sln                    # Archivo de solución de Visual Studio
└── README.md                      # Este archivo
```
