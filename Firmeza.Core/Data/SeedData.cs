using Firmeza.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration; // Necesario para IConfiguration
using Microsoft.Extensions.DependencyInjection; // Necesario para IServiceProvider
using System;
using System.Threading.Tasks;

namespace Firmeza.Core.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Rol>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();

            // --- Crear Roles ---
            string[] roleNames = { "Administrador", "Cliente" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new Rol(roleName));
                }
            }

            // --- Crear Usuario Administrador ---
            var adminEmail = configuration["AdminUser:Email"];
            var adminPassword = configuration["AdminUser:Password"];

            if (adminEmail != null && adminPassword != null)
            {
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new Usuario
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        Nombre = "Administrador del Sistema",
                        Identificacion = "000000000",
                        FechaNacimiento = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(newAdminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdminUser, "Administrador");
                    }
                }
            }
        }
    }
}
