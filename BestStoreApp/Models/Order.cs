using Microsoft.EntityFrameworkCore;

namespace BestStoreApp.Models;

public class Order
{
    public int Id { get; set; }
    public string ClientId { get; set; } = "";
    public virtual ApplicationUser? Client { get; set; }
    public virtual List<OrderItem> Items { get; set; } = [];

    [Precision(16, 2)]
    public decimal ShippingFee { get; set; }

    public string DeliveryAddress { get; set; } = "";
    public string PaymentMethod { get; set; } = "";
    public string PaymentStatus { get; set; } = "";
    public string PaymentDetails { get; set; } = ""; // to store paypal details
    public string OrderStatus { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
