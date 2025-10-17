using Microsoft.EntityFrameworkCore;
using QuickShop.Models;

namespace QuickShop.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {

        }
        // Define your DbSets here. For example:
        // public DbSet<Product> Products { get; set; }
        // public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
