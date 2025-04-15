using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public string UserRank { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRank = roles.FirstOrDefault();
            }
        }
    }
}
