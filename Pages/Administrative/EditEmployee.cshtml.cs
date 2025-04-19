using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Administrative
{

    public class EditEmployeeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public EditEmployeeModel(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public EmployeeViewModel Employee { get; set; }

        public List<string> AllRoles { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage("/Administrative/Employees");
            }

            var roles = await _userManager.GetRolesAsync(user);

            Employee = new EmployeeViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };

            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            return Page(); 
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Employee.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return Page();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var resultRemoveRoles = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!resultRemoveRoles.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove current roles.");
                return Page();
            }

            var resultAddRoles = await _userManager.AddToRolesAsync(user, Employee.Roles);

            if (resultAddRoles.Succeeded)
            {
                return RedirectToPage("/Administrative/Employees");
            }

            ModelState.AddModelError("", "Failed to update roles.");
            return Page();
        }
    }
}