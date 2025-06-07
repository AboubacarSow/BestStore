using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreApp.Models;

public class Product
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public int CategoryId {  get; set; }    
    public virtual Category Category { get; set; } =new Category();
    public string Description { get; set; }=string.Empty;
    [MaxLength(100)]
    public string Brand {  get; set; }=string.Empty;
    [MaxLength(100)]
    public string ImageUrl {  get; set; }=string.Empty;
    public DateTime CreateAt { get; set; }
    [Precision(16,2)]
    public decimal Price { get; set; }
    
}
