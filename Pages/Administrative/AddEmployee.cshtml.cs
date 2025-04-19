using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;

namespace WebApp.Pages.Administrative
{
    public class AddEmployeeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AddEmployeeModel(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public List<IdentityUser> Users { get; set; }

        public List<string> Roles { get; set; }

        [BindProperty]
        public string SelectedUserId { get; set; }

        [BindProperty]
        public string SelectedRole { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();

            if (Users == null || Users.Count == 0)
            {
                Users = new List<IdentityUser>();
            }

            Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(SelectedUserId) || string.IsNullOrEmpty(SelectedRole))
            {
                ModelState.AddModelError("", "User and role must be selected.");
                return Page();
            }

            var user = await _userManager.FindByIdAsync(SelectedUserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return Page();
            }

            var roleExists = await _roleManager.RoleExistsAsync(SelectedRole);
            if (!roleExists)
            {
                ModelState.AddModelError("", "Role does not exist.");
                return Page();
            }

            if (await _userManager.IsInRoleAsync(user, SelectedRole))
            {
                ModelState.AddModelError("", "User already has this role.");
                return Page();
            }

            var result = await _userManager.AddToRoleAsync(user, SelectedRole);
            if (result.Succeeded)
            {
                return RedirectToPage("/Administrative/Employees");
            }

            ModelState.AddModelError("", "Failed to assign role.");
            return Page();
        }
    }
}
