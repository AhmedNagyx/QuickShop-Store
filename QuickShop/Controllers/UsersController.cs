using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuickShop.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QuickShop.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("/[controller]/{action=Index}/{id?}")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly int pageSize = 10;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index(int? pageIndex)
        {
            IQueryable<ApplicationUser> query = userManager.Users.OrderByDescending(x => x.CreatedAt);
            if (pageIndex == null || pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip(((int)pageIndex - 1) * pageSize).Take(pageSize);
            var users = query.ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if(id == null)
            {
               return RedirectToAction("Index","Users");
            }
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Users");
            }
            ViewBag.Roles = await userManager.GetRolesAsync(user);

            // Get all roles
            var allRoles = roleManager.Roles.ToList();
            var items = new List<SelectListItem>();
            foreach (var role in allRoles) {
                items.Add(new SelectListItem { 
                    Text = role.Name,
                    Value = role.Name,
                    Selected = await userManager.IsInRoleAsync(user, role.Name!)
                });
            }
            ViewBag.SelectItems = items;
            return View(user);
        }

        public async Task<IActionResult> EditRole(string? id, string? newRole)
        {
            if(id==null|| newRole == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var roleExists = await roleManager.RoleExistsAsync(newRole);
            var user = await userManager.FindByIdAsync(id);

            if (user == null || !roleExists)
            {
                return RedirectToAction("Index", "Users");
            }

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser!.Id == user.Id)
            {
                TempData["ErrorMessage"] = "You can not update your own role";
                return RedirectToAction("Details", "Users", new { id });
            }
            //update user role
            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, userRoles);
            await userManager.AddToRoleAsync(user, newRole);
            TempData["SuccessMessage"] = "Role updated successfuly";

            return RedirectToAction("Details", "Users", new { id });

        }

        public async Task<IActionResult> DeleteAccount(string? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "Users");
            }
            var appUser = await userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Users");
            }
            var currentUser = await userManager.GetUserAsync(User);
            if(currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You can not delete your own account";
                return RedirectToAction("Details", "Users", new { id });
            }

            var result = await userManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Account deleted successfuly";
                return RedirectToAction("Index", "Users");

            }
           
            
            TempData["ErrorMessage"] = "Error occured while deleting account";
            
            return RedirectToAction("Details", "Users", new { id });

        }
    }
}
