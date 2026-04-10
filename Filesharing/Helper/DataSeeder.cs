namespace Filesharing.Helper;

public static class DataSeeder
{
    private const string AdminEmail = "admin@filesharing.com";
    private const string AdminRole = "Admin";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        var adminUser = await userManager.FindByEmailAsync(AdminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AdminRole);
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, AdminRole))
            {
                await userManager.AddToRoleAsync(adminUser, AdminRole);
            }
        }
    }
}
