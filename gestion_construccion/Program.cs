// Se importan los namespaces necesarios para el funcionamiento de la aplicación.
using gestion_construccion.Datos;
using gestion_construccion.Models;
using gestion_construccion.Repositories;
using gestion_construccion.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

// Se crea el constructor de la aplicación web.
var builder = WebApplication.CreateBuilder(args);

// --- Registro de Servicios en el Contenedor de Inyección de Dependencias ---

// Se añade el servicio para que la aplicación pueda usar controladores y vistas (patrón MVC).
builder.Services.AddControllersWithViews();

// Se registra el DbContext (AppDbContext) en el contenedor de dependencias.
// Esto permite que otras partes de la aplicación (como repositorios y servicios) puedan recibirlo en sus constructores.
builder.Services.AddDbContext<AppDbContext>(options =>
    // Se configura Entity Framework Core para que use el proveedor de PostgreSQL (Npgsql).
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Configuración de ASP.NET Core Identity ---
// Se añade y configura el sistema de Identity.
builder.Services.AddIdentity<Usuario, Rol>(options => {
    // Opciones de configuración para la autenticación y contraseñas.
    options.SignIn.RequireConfirmedAccount = false; // No se requiere que el usuario confirme su email para iniciar sesión.
    // Se relajan los requisitos de la contraseña para facilitar el desarrollo.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>() // Se le dice a Identity que use nuestro AppDbContext para almacenar los datos de usuarios y roles.
.AddDefaultTokenProviders(); // Se añaden los proveedores de tokens por defecto (para reseteo de contraseña, etc.).

// Se configura el comportamiento de la cookie de autenticación.
builder.Services.ConfigureApplicationCookie(options =>
{
    // Si un usuario no autenticado intenta acceder a un recurso protegido, será redirigido a esta ruta.
    options.LoginPath = "/Usuarios/Login";
    // Si un usuario autenticado pero sin el rol adecuado intenta acceder a un recurso, será redirigido aquí.
    options.AccessDeniedPath = "/Usuarios/AccessDenied";
    options.SlidingExpiration = true; // La cookie se renueva automáticamente si el usuario está activo.
});

// --- Registro de Repositorios y Servicios Personalizados ---

// Se registra la UnitOfWork. 'AddScoped' significa que se creará una nueva instancia por cada petición HTTP.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Se registra el repositorio genérico. 'typeof' se usa porque es una clase genérica.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Se registran nuestros servicios de negocio.
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();

// Se construye la aplicación con todos los servicios registrados.
var app = builder.Build();

// --- Sembrado de Datos (Data Seeding) ---
// Este bloque se ejecuta una sola vez al iniciar la aplicación.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Se obtiene la configuración para leer datos de appsettings.json.
        var configuration = services.GetRequiredService<IConfiguration>();
        // Se llama al método estático para inicializar los datos (crear roles y usuario admin si no existen).
        await SeedData.Initialize(services, configuration);
    }
    catch (Exception ex)
    {
        // Si algo falla durante el sembrado, se registra en la consola.
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the DB.");
    }
}

// --- Configuración del Pipeline de Peticiones HTTP ---
// El orden de estos middlewares es muy importante.

// En un entorno que no sea de desarrollo, se usa una página de error genérica.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Fuerza el uso de HTTPS.
}

// app.UseHttpsRedirection(); // Se comenta temporalmente para evitar problemas de redirección en el entorno de desarrollo.

// Permite que la aplicación sirva archivos estáticos (CSS, JavaScript, imágenes) desde la carpeta wwwroot.
app.UseStaticFiles();

// Habilita el sistema de enrutamiento de ASP.NET Core.
app.UseRouting();

// Habilita el middleware de autenticación. Debe ir antes de la autorización.
app.UseAuthentication();
// Habilita el middleware de autorización, que comprueba los roles y políticas.
app.UseAuthorization();

// Se configura la ruta por defecto para el patrón MVC.
// {controller=Home} significa que si no se especifica un controlador, se usará "Home".
// {action=Index} significa que si no se especifica una acción, se usará "Index".
// {id?} significa que el parámetro "id" es opcional.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Se ejecuta la aplicación y empieza a escuchar peticiones HTTP.
app.Run();
