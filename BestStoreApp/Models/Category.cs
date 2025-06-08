using System.ComponentModel.DataAnnotations;

namespace BestStoreApp.Models;

public class Category
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }=string.Empty;
    public virtual IList<Product> Products { get; set; } = [];
  
}
