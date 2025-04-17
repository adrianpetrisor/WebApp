using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Services;

namespace WebApp.Pages.Administrative
{
    [Authorize(Roles = "Administrator,Developer")]
    public class AdminControlsModel : PageModel
    {
        private readonly ILogger<AdminControlsModel> _logger;
        private readonly AuthorizationService _authorizationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string ErrorMessage { get; set; }
        public string StatusMessage { get; set; }
        public string UserRank { get; set; }

        [BindProperty]
        public RoleInputModel Input { get; set; }

        [BindProperty]
        public DeleteInputModel DeleteInput { get; set; }

        public AdminControlsModel(
            ILogger<AdminControlsModel> logger,
            AuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public class RoleInputModel
        {
            public string Username { get; set; }
            public string Role { get; set; }
        }

        public class DeleteInputModel
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ErrorMessage = "You are not authenticated.";
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);
            UserRank = roles.FirstOrDefault() ?? "None";
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

            var targetRoles = await _userManager.GetRolesAsync(targetUser);
            if (targetRoles.Contains(Input.Role, StringComparer.OrdinalIgnoreCase))
            {
                StatusMessage = $"User '{Input.Username}' is already in the '{Input.Role}' role.";
                await OnGetAsync();
                return Page();
            }

            // Remove existing roles
            var removeResult = await _userManager.RemoveFromRolesAsync(targetUser, targetRoles);
            if (!removeResult.Succeeded)
            {
                StatusMessage = $"Failed to remove existing roles: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}";
                await OnGetAsync();
                return Page();
            }

            // Check role exists
            var roleExists = await _roleManager.RoleExistsAsync(Input.Role.ToUpper());
            if (!roleExists)
            {
                StatusMessage = $"Role '{Input.Role}' does not exist in the database.";
                await OnGetAsync();
                return Page();
            }

            // Assign new role
            var result = await _userManager.AddToRoleAsync(targetUser, Input.Role);
            StatusMessage = result.Succeeded
                ? $"Successfully assigned role '{Input.Role}' to user '{Input.Username}'."
                : $"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}";

            _logger.LogInformation($"User '{User.Identity.Name}' changed role of '{Input.Username}' to '{Input.Role}'.");

            await OnGetAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostFindUserAsync()
        {
            if (string.IsNullOrWhiteSpace(DeleteInput.Username))
            {
                StatusMessage = "Please enter a username.";
                await OnGetAsync();
                return Page();
            }

            var user = await _userManager.FindByNameAsync(DeleteInput.Username);
            if (user == null)
            {
                StatusMessage = $"User '{DeleteInput.Username}' not found.";
                await OnGetAsync();
                return Page();
            }

            var roles = await _userManager.GetRolesAsync(user);
            DeleteInput.Email = user.Email;
            DeleteInput.Role = roles.FirstOrDefault() ?? "None";

            await OnGetAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync()
        {
            if (string.IsNullOrWhiteSpace(DeleteInput.Username))
            {
                StatusMessage = "Invalid username.";
                await OnGetAsync();
                return Page();
            }

            var user = await _userManager.FindByNameAsync(DeleteInput.Username);
            if (user == null)
            {
                StatusMessage = $"User '{DeleteInput.Username}' not found.";
                await OnGetAsync();
                return Page();
            }

            var result = await _userManager.DeleteAsync(user);
            StatusMessage = result.Succeeded
                ? $"User '{DeleteInput.Username}' has been deleted successfully."
                : $"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}";

            _logger.LogInformation($"User '{User.Identity.Name}' deleted account '{DeleteInput.Username}'.");

            await OnGetAsync();
            return Page();
        }
    }
}
