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
        private readonly UserManager<IdentityUser> _userManager;

        public string ErrorMessage { get; set; }
        public string StatusMessage { get; set; }
        public string UserRank { get; set; }

        [BindProperty]
        public DeleteInputModel DeleteInput { get; set; }

        public AdminControlsModel(
            ILogger<AdminControlsModel> logger,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
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
