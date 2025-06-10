using System.ComponentModel.DataAnnotations;

namespace BestStoreApp.Models;

public class PaymentDetails
{
    [Key]
    public string Id { get; set; }
    public string StripeChargeId { get; set; } = string.Empty;
    public long? Amount { get; set; }   
    public string Currency { get; set; }=string.Empty;
    public string CardLast4Digits {  get; set; } = string.Empty;
    public string ? Status { get; set; }  
    public DateTime? PaidAt { get; set; }
    public string Method { get; set; } = string.Empty;
}
