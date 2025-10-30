using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            return View(user);
        }
    }
}
