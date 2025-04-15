using Microsoft.AspNetCore.Identity;

namespace WebApp.Services
{
    public class AuthorizationService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthorizationService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> IsAuthorizedAsync(IdentityUser user, string requiredRole)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.Contains(requiredRole);
        }
    }
}
