using Microsoft.AspNetCore.Identity;

namespace WebApp.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Administrator", "Quality Assurance", "User", "Human Resources", "Developer", "Employee" };

            var existingRoles = roleManager.Roles.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var roleName in roleNames)
            {
                if (!existingRoles.Contains(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
