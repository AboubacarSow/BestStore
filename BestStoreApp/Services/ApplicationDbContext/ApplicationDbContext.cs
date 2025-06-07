using BestStoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreApp.Services.ApplicationDbContext;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       optionsBuilder.UseLazyLoadingProxies();  
    }
    public DbSet<Product> Products { get; set; }  
    public DbSet<Category> Categories { get; set; }

}
