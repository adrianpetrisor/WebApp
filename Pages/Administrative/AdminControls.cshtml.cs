using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Services;

namespace WebApp.Pages.Administrative
{
    [Authorize(Roles = "Administrator")]
    public class AdminControlsModel : PageModel
    {
        private readonly ILogger<AdminControlsModel> _logger;
        private readonly AuthorizationService _authorizationService;
        private readonly UserManager<IdentityUser> _userManager;

        public string ErrorMessage { get; set; }
        public string UserRank { get; set; }

        public AdminControlsModel(ILogger<AdminControlsModel> logger, AuthorizationService authorizationService, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                ErrorMessage = "You are not authenticated. Please authenticate and try again!";
                return;
            }

            var isAuthorized = await _authorizationService.IsAuthorizedAsync(user, "Administrator");
            var roles = await _userManager.GetRolesAsync(user);
            UserRank = roles.FirstOrDefault();

            if (!isAuthorized)
            {
                ErrorMessage = "You don't have permission to use this panel.";
            }
        }
    }
}
