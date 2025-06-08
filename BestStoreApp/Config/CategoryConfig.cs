using BestStoreApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BestStoreApp.Config
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
                new Category {Id=1, Name = "Computers" },
                new Category {Id=2, Name = "Accessories" },
                new Category {Id=3, Name = "Printers" },
                new Category {Id=4, Name = "Cameras" },
                new Category {Id=5, Name = "Other" },
                new Category {Id=6, Name = "Phones" }
                );
        }
    }
}
