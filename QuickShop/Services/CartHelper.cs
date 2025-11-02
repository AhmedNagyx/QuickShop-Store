using QuickShop.Models;
using System.Text.Json;

namespace QuickShop.Services
{
    public class CartHelper
    {
        public static Dictionary<int,int> GetCartDictionary(HttpRequest request,HttpResponse response)
        {
            string cartCookieValue = request.Cookies["ShoopingCartCookie"] ?? "";
            try
            {
                var cart = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cartCookieValue));
                var dictionary = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
                if (dictionary != null)
                {
                    return dictionary;
                }
            }
            catch(Exception)
            {

            }

            if (cartCookieValue.Length > 0)
            {
                response.Cookies.Delete("ShoopingCartCookie");
            }
            return new Dictionary<int, int>();
        }

        public static int GetCartSize(HttpRequest request, HttpResponse response)
        {
            var cartDictionary = GetCartDictionary(request, response);
            int size = 0;
            foreach (var item in cartDictionary)
            {
                size += item.Value;
            }
            return size;
        }

        public static List<OrderItem> GetCartItems(HttpRequest request, HttpResponse response, ApplicationDbContext context)
        {
            var cartItems = new List<OrderItem>();
            var cartDictionary = GetCartDictionary(request, response);
            foreach(var item in cartDictionary)
            {
                int productId = item.Key;
                int quantity = item.Value;
                var product = context.Products.Find(productId);
                if (product == null) continue;
                var orderItem = new OrderItem
                {
                    
                    Product = product,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };

                cartItems.Add(orderItem);
            }
            return cartItems;
        }

        public static decimal GetSubTotal(List<OrderItem> cartItems)
        {
            decimal subTotal = 0;
            foreach (var item in cartItems)
            {
                subTotal += item.UnitPrice * item.Quantity;
            }
            return subTotal;    
        }
    }
}
