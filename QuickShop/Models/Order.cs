using Microsoft.EntityFrameworkCore;

namespace QuickShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        //foreign key
        public string? ClientId { get; set; }
        public ApplicationUser Client { get; set; } = null!;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        [Precision(16,2)]
        public decimal ShippingFee { get; set; }
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
        public string PaymentDetails { get; set; } = "";
        public string OrderStatus { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
