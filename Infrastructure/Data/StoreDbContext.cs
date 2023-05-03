using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
        }

        public DbSet<Product> products { get; set; }
        public DbSet<ProductType> productTypes { get; set; }
        public DbSet<ProductBrand> productBrands { get; set; }

    }
}
