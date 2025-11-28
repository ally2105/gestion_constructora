# Proyecto Firmeza: Sistema de Gestión de Construcción

## 🏗️ Descripción General

**Firmeza** es un sistema integral para la gestión de productos y ventas en el sector de la construcción. Está desarrollado con una arquitectura de microservicios que incluye una API REST, un frontend para clientes (SPA) y un panel de administración web moderno y profesional.

El sistema permite a los clientes registrarse, iniciar sesión, explorar un catálogo de productos, gestionar un carrito de compras, realizar pedidos y recibir confirmaciones por correo electrónico con el comprobante de venta en formato PDF. El panel de administración ofrece funcionalidades completas para gestionar productos, clientes y ventas, incluyendo la importación masiva de datos desde archivos Excel.

## ✨ Características Principales

### Frontend de Clientes (React SPA)
- **Autenticación JWT:** Registro e inicio de sesión seguro con tokens JWT.
- **Catálogo de Productos:** Visualización paginada de productos con diseño corporativo moderno.
- **Carrito de Compras:** Gestión de productos seleccionados, cálculo automático de totales (subtotal, IVA, total).
- **Proceso de Compra:** Envío de pedidos a la API con validación en tiempo real.
- **Notificaciones:** Mensajes de éxito/error mediante "toast" no intrusivos.
- **Diseño UI/UX Premium:** Interfaz moderna con glassmorphism, gradientes y animaciones suaves.
- **Responsive Design:** Totalmente adaptable a dispositivos móviles y tablets.

### Backend (ASP.NET Core API REST)
- **Autenticación y Autorización:** Basada en JWT y roles (Cliente, Administrador).
- **Gestión de Usuarios:** Integración completa con ASP.NET Core Identity.
- **Gestión de Productos:** CRUD completo con control de stock.
- **Gestión de Clientes:** CRUD completo con validación de datos.
- **Gestión de Ventas:** Creación de ventas con detalles asociados y actualización automática de inventario.
- **Generación de PDF:** Creación automática de comprobantes de venta en formato PDF usando iTextSharp.
- **Servicio de Correo:** Envío de correos electrónicos (confirmaciones, comprobantes) utilizando SMTP real (Gmail/MailKit).
- **API Documentation:** Swagger/OpenAPI para documentación interactiva de la API.
- **Health Checks:** Endpoint `/health` para monitoreo del estado de la aplicación.

### Panel de Administración (ASP.NET Core MVC)
- **Autenticación Segura:** Inicio de sesión exclusivo para administradores con protección de rutas.
- **Dashboard Interactivo:** Visualización de métricas clave (productos, clientes, ventas) con diseño moderno.
- **Gestión de Productos:** CRUD completo con indicadores visuales de stock y validación.
- **Gestión de Clientes:** CRUD completo con búsqueda y filtrado.
- **Gestión de Ventas:** Visualización de ventas con generación y descarga de PDFs.
- **Importación de Datos:** Importación masiva de productos desde archivos Excel con validación.
- **Diseño UI/UX Premium:** Interfaz profesional con iconos SVG, glassmorphism y animaciones.
- **Responsive Design:** Adaptable a diferentes tamaños de pantalla.

## 🏛️ Arquitectura del Proyecto

El proyecto sigue una **arquitectura en capas** con separación de responsabilidades:

### Firmeza.Core
- **Modelos de Dominio:** Entidades principales (Usuario, Cliente, Producto, Venta, etc.)
- **Interfaces:** Contratos para repositorios y servicios
- **DTOs:** Objetos de transferencia de datos
- **Data Context:** Configuración de Entity Framework Core

### Firmeza.Infrastructure
- **Repositorios:** Implementación del patrón Repository y Unit of Work
- **Servicios:** Lógica de negocio (ProductoService, ClienteService, VentaService, etc.)
- **PdfService:** Generación de documentos PDF
- **EmailService:** Envío de correos electrónicos

### gestion_construcion.api
- **Controllers:** Endpoints REST para la API
- **Autenticación JWT:** Configuración de tokens y validación
- **CORS:** Configuración para permitir peticiones desde el frontend
- **Swagger:** Documentación interactiva de la API

### gestion_construccion.web
- **Controllers:** Controladores MVC para el panel de administración
- **Views:** Vistas Razor con diseño moderno
- **wwwroot:** Archivos estáticos (CSS, JavaScript, imágenes, PDFs generados)

### Firmeza.Client
- **React SPA:** Aplicación de página única para clientes
- **Context API:** Gestión de estado global (autenticación, carrito)
- **Axios:** Cliente HTTP para comunicación con la API
- **React Router:** Navegación entre páginas

### Firmeza.Tests
- **Pruebas Unitarias:** Tests con xUnit y Moq
- **Cobertura:** Tests para servicios y controladores principales

## 🚀 Tecnologías Utilizadas

### Frontend
- **React 18:** Librería para construir interfaces de usuario
- **Vite:** Herramienta de construcción rápida para proyectos frontend
- **Axios:** Cliente HTTP para comunicación con la API
- **React Router DOM v6:** Para la navegación en la SPA
- **jwt-decode:** Para decodificar tokens JWT en el cliente
- **react-hot-toast:** Para notificaciones "toast"
- **CSS Moderno:** Variables CSS, Flexbox, Grid, Glassmorphism

### Backend
- **.NET 8:** Framework para construir la API REST y el panel de administración
- **Entity Framework Core 8:** ORM para interacción con PostgreSQL
- **PostgreSQL 16:** Base de datos relacional
- **ASP.NET Core Identity:** Sistema de gestión de usuarios y roles
- **JWT (JSON Web Tokens):** Para autenticación segura
- **AutoMapper:** Para mapeo de objetos entre DTOs y entidades
- **MailKit:** Librería para el envío de correos electrónicos vía SMTP
- **iTextSharp:** Librería para la generación de documentos PDF
- **xUnit:** Framework para pruebas unitarias
- **Moq:** Librería para simulación de objetos en pruebas

### DevOps
- **Docker:** Para la contenedorización de la aplicación
- **Docker Compose:** Para la orquestación de los servicios
- **Multi-stage Builds:** Optimización de imágenes Docker

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
   ```bash
   docker compose up --build
   ```
   
   Este comando realizará las siguientes acciones:
   1. Construirá las imágenes de Docker para cada servicio
   2. Ejecutará las pruebas unitarias
   3. Si las pruebas pasan, levantará los contenedores para:
      - PostgreSQL (Base de datos)
      - API REST
      - Panel de Administración
      - Frontend de Clientes

3. **Accede a los servicios:**
   - **Frontend de Clientes:** `http://localhost:3000`
   - **Panel de Administración:** `http://localhost:5037`
   - **API (Swagger):** `http://localhost:5165/swagger`
   - **API Health Check:** `http://localhost:5165/health`

4. **Detener los servicios:**
   ```bash
   docker compose down
   ```

## 🛠️ Ejecución en Local (Sin Docker)

Si prefieres ejecutar los servicios localmente sin Docker, sigue estos pasos.

### Prerrequisitos
- **.NET SDK 8.0:** [Descargar .NET](https://dotnet.microsoft.com/download)
- **Node.js 18+ y npm:** [Descargar Node.js](https://nodejs.org/)
- **PostgreSQL 14+:** Servidor de base de datos

### Configuración

1. **Base de Datos (Clever Cloud PostgreSQL):**
   - El proyecto está configurado para usar una base de datos PostgreSQL alojada en Clever Cloud.
   - Las credenciales ya están configuradas en los archivos `appsettings.json` y `appsettings.Development.json`.
   - Si deseas usar una base de datos local, actualiza la cadena de conexión `DefaultConnection` en los archivos de configuración:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=gestion_constructora;Username=tu_usuario;Password=tu_contraseña"
     }
     ```

2. **Servicio de Correo (Gmail SMTP):**
   - En los archivos `appsettings.json`, actualiza la sección `SmtpSettings`:
     ```json
     "SmtpSettings": {
       "Server": "smtp.gmail.com",
       "Port": 587,
       "SenderName": "Firmeza",
       "SenderEmail": "tu_email@gmail.com",
       "Username": "tu_email@gmail.com",
       "Password": "tu_contraseña_de_aplicacion"
     }
     ```
   - **Nota:** Usa una [Contraseña de Aplicación](https://support.google.com/accounts/answer/185833) de Gmail, no tu contraseña normal.

3. **Usuario Administrador:**
   - En los archivos `appsettings.json`, configura el usuario administrador inicial:
     ```json
     "AdminUser": {
       "Email": "admin@firmeza.com",
       "Password": "Admin123!",
       "Nombre": "Administrador"
     }
     ```

### Ejecución

1. **Aplicar Migraciones:**
   ```bash
   cd gestion_construcion.api
   dotnet ef database update
   ```

2. **Iniciar la API:**
   ```bash
   cd gestion_construcion.api
   dotnet run
   ```
   La API estará disponible en `http://localhost:5165`

3. **Iniciar el Panel de Administración:**
   ```bash
   cd gestion_construccion.web
   dotnet run
   ```
   El panel estará disponible en `http://localhost:5037`

4. **Iniciar el Frontend de Clientes:**
   ```bash
   cd Firmeza.Client
   npm install
   npm run dev
   ```
   El frontend estará disponible en `http://localhost:3000`

## 🔑 Credenciales por Defecto

### Panel de Administración
- **Usuario:** `admin@firmeza.com`
- **Contraseña:** `Admin123!`

### Frontend de Clientes
- Puedes registrar nuevos usuarios desde la página de registro
- Los nuevos usuarios tienen el rol de "Cliente" automáticamente

## 📂 Estructura del Proyecto

```
Firmeza/
├── Firmeza.Core/                  # Contratos, Modelos de Dominio, DTOs
│   ├── Data/                      # DbContext y configuraciones de EF Core
│   ├── DTOs/                      # Data Transfer Objects
│   ├── Interfaces/                # Interfaces de repositorios y servicios
│   └── Models/                    # Entidades del dominio
├── Firmeza.Infrastructure/        # Implementación de repositorios y servicios
│   ├── Repositories/              # Implementación del patrón Repository
│   └── Services/                  # Lógica de negocio (Email, PDF, etc.)
├── Firmeza.Client/                # Frontend de Clientes (React SPA)
│   ├── src/
│   │   ├── components/            # Componentes reutilizables
│   │   ├── context/               # Context API (Auth, Cart)
│   │   ├── pages/                 # Páginas de la aplicación
│   │   └── services/              # Servicios de API
│   └── public/                    # Archivos estáticos
├── gestion_construcion.api/       # Backend (ASP.NET Core API REST)
│   ├── Controllers/               # Controladores de la API
│   └── Properties/                # Configuración de lanzamiento
├── gestion_construccion.web/      # Panel de Administración (ASP.NET Core MVC)
│   ├── Controllers/               # Controladores MVC
│   ├── Views/                     # Vistas Razor
│   ├── wwwroot/                   # Archivos estáticos (CSS, JS, PDFs)
│   └── Models/                    # ViewModels
├── Firmeza.Tests/                 # Pruebas Unitarias (xUnit)
│   └── Services/                  # Tests de servicios
├── docker-compose.yml             # Orquestación de contenedores
├── docker-compose.no-client.yml   # Compose sin el frontend
├── Dockerfile                     # Dockerfile multi-stage
└── README.md                      # Este archivo
```

## 🧪 Pruebas

El proyecto incluye pruebas unitarias con xUnit y Moq.

### Ejecutar las pruebas localmente:
```bash
dotnet test
```

### Ejecutar las pruebas en Docker:
Las pruebas se ejecutan automáticamente durante el `docker compose up --build`.

## 🎨 Diseño y UI/UX

El proyecto implementa un diseño moderno y profesional con:

- **Paleta de colores corporativa:** Tonos de azul (#2563eb, #1e40af) y verde (#10b981)
- **Glassmorphism:** Efectos de vidrio esmerilado en tarjetas y modales
- **Gradientes:** Fondos con gradientes suaves
- **Iconos SVG:** Iconos vectoriales escalables para mejor rendimiento
- **Animaciones:** Transiciones suaves y micro-interacciones
- **Tipografía:** Fuentes modernas y legibles
- **Responsive:** Adaptable a todos los tamaños de pantalla

## 🔒 Seguridad

- **Autenticación JWT:** Tokens seguros con expiración configurable
- **Protección de rutas:** Middleware de autorización en API y panel de administración
- **Validación de datos:** Validación en cliente y servidor
- **Protección CSRF:** Anti-forgery tokens en formularios MVC
- **Passwords hasheados:** ASP.NET Core Identity con hashing seguro
- **CORS configurado:** Solo permite peticiones desde orígenes autorizados

## 📝 Mejores Prácticas Implementadas

- **Arquitectura en capas:** Separación clara de responsabilidades
- **Patrón Repository:** Abstracción de acceso a datos
- **Unit of Work:** Gestión de transacciones
- **Dependency Injection:** Inyección de dependencias en toda la aplicación
- **DTOs:** Separación entre modelos de dominio y transferencia
- **Async/Await:** Operaciones asíncronas para mejor rendimiento
- **Logging:** Registro de eventos y errores
- **Comentarios XML:** Documentación en código
- **Código limpio:** Sin duplicación, bien organizado

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor, abre un issue o pull request para sugerencias o mejoras.

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

## 👨‍💻 Autor

Desarrollado como proyecto de gestión de construcción con tecnologías modernas.

---

**¿Necesitas ayuda?** Revisa la documentación de Swagger en `/swagger` o contacta al equipo de desarrollo.
