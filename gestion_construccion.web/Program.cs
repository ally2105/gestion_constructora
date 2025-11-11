// Se importan los namespaces necesarios para el funcionamiento de la aplicación.
using gestion_construccion.web.Datos;
using gestion_construccion.web.Models;
using gestion_construccion.web.Repositories;
using gestion_construccion.web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de Servicios en el Contenedor de Inyección de Dependencias ---

// Se añade el servicio para que la aplicación pueda usar controladores y vistas (patrón MVC).
builder.Services.AddControllersWithViews();

// Se registra la FÁBRICA de DbContext. Esto es ideal para servicios de larga duración como la importación.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Se registra el DbContext como Scoped para el resto de la aplicación (controladores, etc.).
// 'AddScoped' significa que se creará una nueva instancia por cada petición HTTP.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Configuración de ASP.NET Core Identity ---
// Se añade y configura el sistema de Identity para manejar usuarios y roles.
builder.Services.AddIdentity<Usuario, Rol>(options => {
    // Opciones de configuración para la autenticación y contraseñas.
    options.SignIn.RequireConfirmedAccount = false; // No se requiere que el usuario confirme su email.
    // Se relajan los requisitos de la contraseña para facilitar el desarrollo.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>() // Se le dice a Identity que use nuestro AppDbContext para almacenar los datos.
.AddDefaultTokenProviders(); // Se añaden los proveedores de tokens por defecto (para reseteo de contraseña, etc.).

// Se configura el comportamiento de la cookie de autenticación.
builder.Services.ConfigureApplicationCookie(options =>
{
    // Si un usuario no autenticado intenta acceder a un recurso protegido, será redirigido a esta ruta.
    options.LoginPath = "/Usuarios/Login";
    // Si un usuario autenticado pero sin el rol adecuado intenta acceder, será redirigido aquí.
    options.AccessDeniedPath = "/Usuarios/AccessDenied";
    options.SlidingExpiration = true; // La cookie se renueva automáticamente si el usuario está activo.
});

// --- Registro de Repositorios y Servicios Personalizados ---

// Se registra la UnitOfWork.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Se registra el repositorio genérico. 'typeof' se usa porque es una clase genérica.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Se registran nuestros servicios de negocio para que puedan ser inyectados en los controladores.
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddTransient<IEmailService, SmtpEmailService>(); // <-- LÍNEA AÑADIDA

// Se añade el servicio de Logging para poder inyectar ILogger en los servicios.
builder.Services.AddLogging();

// Se construye la aplicación con todos los servicios registrados.
var app = builder.Build();

// --- Sembrado de Datos (Data Seeding) ---
// Este bloque se ejecuta una sola vez al iniciar la aplicación.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Se obtiene la configuración para leer datos de appsettings.json (como el usuario admin).
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
    app.UseHsts(); // Fuerza el uso de HTTPS en producción.
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
// {controller=Home}/{action=Index}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Se ejecuta la aplicación y empieza a escuchar peticiones HTTP.
app.Run();
