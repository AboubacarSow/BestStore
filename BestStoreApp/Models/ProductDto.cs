using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreApp.Models;

public class ProductDto
{
    [Required,MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage ="Category of product is required")]
    public int CategoryId { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required,MaxLength(100)]
    public string Brand { get; set; } = string.Empty;
    public IFormFile? ImageFile { get; set; }
    [Required]
    public decimal Price { get; set; }
}
