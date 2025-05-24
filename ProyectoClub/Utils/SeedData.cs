using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoClub.Models; // Asegúrate de que tu modelo Usuario esté en este namespace
using System;
using System.Threading.Tasks;

namespace ProyectoClub.Utils
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Crear Roles si no existen
            string[] roleNames = { "Admin", "Miembro" }; // Define los roles que quieres

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Crear el rol y guardarlo en la DB
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Crear un usuario Administrador si no existe
            var adminUserEmail = "admin@proyecto.com"; // Email para el usuario administrador
            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);

            if (adminUser == null)
            {
                var newAdminUser = new Usuario
                {
                    UserName = adminUserEmail,
                    Email = adminUserEmail,
                    EmailConfirmed = true, // Confirmar el email para que pueda loguearse directamente
                    Nombres = "Admin",
                    Apellidos = "Admin",
                    Direccion = "Direccion del Admin",
                };

                var createPowerUser = await userManager.CreateAsync(newAdminUser, "Admin123!"); // Contraseña para el admin
                if (createPowerUser.Succeeded)
                {
                    // Asignar el rol "Admin" al nuevo usuario
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
                else
                {
                    // Manejar errores si la creación del usuario falla (opcional)
                    // Puedes loguear createPowerUser.Errors
                    Console.WriteLine("Error al crear el usuario administrador: " + string.Join(", ", createPowerUser.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
