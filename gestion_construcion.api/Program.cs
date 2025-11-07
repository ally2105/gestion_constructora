// Se importan los namespaces del proyecto principal y de los paquetes necesarios.
using gestion_construccion.web.Datos;
using gestion_construccion.web.Models;
using gestion_construccion.web.Repositories;
using gestion_construccion.web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de Servicios en el Contenedor de Inyección de Dependencias ---

// Se añaden los servicios básicos para controladores de API.
builder.Services.AddControllers();

// Se añaden los servicios para la generación de la documentación de la API (Swagger).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Se registra el DbContext (AppDbContext) de la misma forma que en el proyecto principal.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Configuración de ASP.NET Core Identity ---
// Se añade y configura el sistema de Identity, exactamente igual que en el proyecto principal.
builder.Services.AddIdentity<Usuario, Rol>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// --- Registro de Repositorios y Servicios Personalizados ---
// Se registran la UnitOfWork, el Repositorio genérico y nuestros servicios de negocio.
// Esto permite que los controladores de la API puedan inyectarlos y usarlos.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();

var app = builder.Build();

// --- Configuración del Pipeline de Peticiones HTTP ---

// En desarrollo, se habilita la interfaz de Swagger para probar la API.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Se habilita la autorización (necesario si queremos proteger endpoints en el futuro).
app.UseAuthorization();

// Se mapean los controladores de la API.
app.MapControllers();

// Se ejecuta la aplicación.
app.Run();
