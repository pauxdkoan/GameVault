using GameVault.Source.Domain.Contants;
using GameVault.Source.Domain.Entities;
using Microsoft.AspNetCore.Identity;



namespace GameVault.Source.Infrastructure.Seeds
{
    public class IdentitySeeder
    {
     
        public static async Task SeedAsync( UserManager<ApplicationUser> userManager,RoleManager<IdentityRole<Guid>> roleManager)
        {

            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager);
        }
        private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {

            foreach (var roleName in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole<Guid> { Name = roleName };
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                        ", ",
                        result.Errors.Select(error => error.Description));

                        throw new InvalidOperationException(
                            $"No se pudo crear el rol {roleName}: {errors}");
                    }
                }



            }

        }

        private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser defaultAdmin = new()
            {
                DisplayName="Paulillo",
                Email = "paulo@email.com",
                UserName = "paulo@email.com",
                IsActive = true,
                EmailConfirmed = true,



            };

            var existingAdmin =  await userManager.FindByEmailAsync(defaultAdmin.Email);
            if (existingAdmin != null) return;
            
            var result=await userManager.CreateAsync(defaultAdmin, "Paulo123!");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el administrador: {errors}");
            }

            await userManager.AddToRoleAsync( defaultAdmin, Roles.Administrator);
        }
    }
}
