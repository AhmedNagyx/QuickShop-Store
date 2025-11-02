using System.ComponentModel.DataAnnotations;

namespace QuickShop.Models
{
    public class CheckoutDto
    {
        [Required(ErrorMessage ="Delivery address is required")]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
