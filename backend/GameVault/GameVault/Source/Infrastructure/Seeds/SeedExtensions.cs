using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Seeds;
using Microsoft.AspNetCore.Identity;

namespace FixFlowApp.Source.Infrastructure.Seeds
{
    public static class SeedExtensions
    {
        public static async Task SeedIdentityAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var configuration = services.GetRequiredService<IConfiguration>();

                await IdentitySeeder.SeedAsync(userManager, roleManager, configuration);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Crea los datos de ejemplo (20 usuarios con biblioteca, reseñas y listas).
        /// Se desactiva con SeedSettings:DemoData = false (en Azure: SeedSettings__DemoData).
        /// Si falla (por ejemplo, IGDB no responde) la API arranca igual y lo
        /// vuelve a intentar en el siguiente arranque.
        /// </summary>
        public static async Task SeedDemoDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();

            if (!configuration.GetValue("SeedSettings:DemoData", true))
            {
                return;
            }

            try
            {
                var seeder = ActivatorUtilities.CreateInstance<DemoDataSeeder>(services);
                await seeder.SeedAsync();
            }
            catch (Exception exception)
            {
                services.GetRequiredService<ILogger<DemoDataSeeder>>()
                    .LogError(exception, "No se pudieron crear los datos de ejemplo.");
            }
        }
    }
}
