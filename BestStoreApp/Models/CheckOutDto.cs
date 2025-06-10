using System.ComponentModel.DataAnnotations;

namespace BestStoreApp.Models;

public class CheckOutDto
{
    [Required(ErrorMessage = "The Delivery Address is required.")]
    [MaxLength(200)]
    public string DeliveryAddress { get; set; } = "";
    public string PaymentMethod { get; set; } = "";
}   