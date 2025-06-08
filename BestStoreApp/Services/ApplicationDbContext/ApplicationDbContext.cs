using BestStoreApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BestStoreApp.Services.ApplicationDbContext;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }  
    public DbSet<Category> Categories { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       optionsBuilder.UseLazyLoadingProxies();  
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(c=>c.Category)
            .WithMany(p => p.Products)
            .HasForeignKey(c=>c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(AssemblyReference)));
    }

}
