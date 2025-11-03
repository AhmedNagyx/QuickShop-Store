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

        public IActionResult Confirm()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal total = CartHelper.GetSubTotal(cartItems) + shippingFee;
            int CartSize = 0;
            foreach (var item in cartItems)
            {
                CartSize += item.Quantity;
            }

            string deliveryAddress = TempData["DeliverryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();

            if(CartSize==0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index","Home");
            }

            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.Total = total;
            ViewBag.CartSize = CartSize;

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmAsync(int anything)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            string deliveryAddress = TempData["DeliverryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();
            if (cartItems.Count == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var appUser = await userManager.GetUserAsync(User);
            if(appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //save order
            var order = new Order
            {
                ClientId = appUser.Id,
                OrderItems = cartItems,
                ShippingFee = shippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Pending",
                PaymentDetails = "",
                OrderStatus = "Created",
                CreatedAt = DateTime.UtcNow

            };
            context.Orders.Add(order);
            context.SaveChanges();

            //delete cookie
            Response.Cookies.Delete("ShoopingCartCookie");

            ViewBag.SuccessMessage = "Order created successfully";


            return View();
        }
    }
}
