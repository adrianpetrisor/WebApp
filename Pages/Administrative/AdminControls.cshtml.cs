using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Administrative
{
    [Authorize(Roles = "Administrator,Developer")]
    public class AdminControlsModel : PageModel
    {
        private readonly ILogger<AdminControlsModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public string StatusMessage { get; set; }
        public string UserRank { get; set; }

        [BindProperty]
        public RoleInputModel Input { get; set; }

        public AdminControlsModel(
            ILogger<AdminControlsModel> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public class RoleInputModel
        {
            public string Username { get; set; }
            public string Role { get; set; }
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRank = roles.FirstOrDefault() ?? "None";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Invalid input!";
                await OnGetAsync();
                return Page();
            }

            var targetUser = await _userManager.FindByNameAsync(Input.Username);
            if (targetUser == null)
            {
                StatusMessage = $"User '{Input.Username}' not found.";
                await OnGetAsync();
                return Page();
            }

            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == Input.Role);
            if (role == null)
            {
                StatusMessage = $"Role '{Input.Role}' does not exist in the database.";
                await OnGetAsync();
                return Page();
            }

            var currentRoles = await _userManager.GetRolesAsync(targetUser);

            if (currentRoles.Any(role => role.Equals(Input.Role, StringComparison.OrdinalIgnoreCase)))
            {
                StatusMessage = $"User '{Input.Username}' is already in the '{Input.Role}' role.";
                await OnGetAsync();
                return Page();
            }

            var removeResult = await _userManager.RemoveFromRolesAsync(targetUser, currentRoles);

            if (!removeResult.Succeeded)
            {
                StatusMessage = "Failed to remove the existing roles from the user.";
                await OnGetAsync();
                return Page();
            }

            var addResult = await _userManager.AddToRoleAsync(targetUser, Input.Role);

            if (addResult.Succeeded)
            {
                StatusMessage = $"Successfully changed role to '{Input.Role}' for user '{Input.Username}'.";
                _logger.LogInformation($"User '{User.Identity.Name}' changed role to '{Input.Role}' for '{Input.Username}'.");
            }
            else
            {
                StatusMessage = "Something went wrong: " + string.Join("; ", addResult.Errors.Select(e => e.Description));
            }

            await OnGetAsync();
            return Page();
        }





    }
}
