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

                await IdentitySeeder.SeedAsync(userManager, roleManager);
            }
            catch (Exception)
            {
            }
        }
    }
}
