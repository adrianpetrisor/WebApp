using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Pages.Administrative
{
    public class EmployeesModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeesModel(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<EmployeeViewModel> Employees { get; set; }

        public bool CanAddEmployee { get; set; }

        public async Task OnGetAsync()
        {
            Employees = new List<EmployeeViewModel>();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                Employees.Add(new EmployeeViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            CanAddEmployee = currentUserRoles.Contains("Administrator") ||
                             currentUserRoles.Contains("Human Resources") ||
                             currentUserRoles.Contains("Developer");
        }
    }
}
