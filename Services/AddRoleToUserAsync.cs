using Microsoft.AspNetCore.Identity;

public class RoleService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> AddRoleToUserAsync(string username, string roleName)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return false;

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded) return false;
        }

        var roleAssignResult = await _userManager.AddToRoleAsync(user, roleName);
        return roleAssignResult.Succeeded;
    }
}
