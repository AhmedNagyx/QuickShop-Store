using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickShop.Models;
using QuickShop.Services;

namespace QuickShop.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 10;
        public AdminOrdersController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int pageIndex)
        {
            IQueryable<Order> query = context.Orders.Include(o => o.Client).Include(o => o.OrderItems).OrderByDescending(o => o.Id);
            

            if(pageIndex <= 0)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var orders = query.ToList();

            ViewBag.Orders = orders;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;
            return View();
        }

        public IActionResult Details(int id)
        {
            var order = context.Orders.Include(o => o.Client)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.NumberOfOrders = context.Orders.Where(o=>o.ClientId == order.ClientId).Count();
            return View(order);
        }
    }
}
