using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Services;

namespace QuickShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly decimal shippingFee;

        public CartController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request ,Response, context);
            decimal subtotal = CartHelper.GetSubTotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.Subtotal = subtotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Total = subtotal + shippingFee;

            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckoutDto checkoutDto)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal subtotal = CartHelper.GetSubTotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.Subtotal = subtotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Total = subtotal + shippingFee;

            if (!ModelState.IsValid)
            {
                return View(checkoutDto);
            }
            if(cartItems.Count == 0)
            {
                ViewBag.ErrorMessage = "Your cart is empty. Please add items to your cart before checking out.";
                return View(checkoutDto);
            }

            TempData["DeliverryAddress"] = checkoutDto.DeliveryAddress;
            TempData["PaymentMethod"] = checkoutDto.PaymentMethod;

            return RedirectToAction("Confirm");
        }
    }
}
