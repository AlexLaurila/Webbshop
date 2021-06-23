using Microsoft.EntityFrameworkCore;
using WoM_Labb1.Models;

namespace WoM_Labb1.Data
{
    public class ProduktContext : DbContext
    {
        public ProduktContext(DbContextOptions<ProduktContext> options)
            : base(options)
        {          
        }

        public DbSet<Produkt> Produkt { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<ShoppingCart> ShoppingCart { get; set; }

        public DbSet<WoM_Labb1.Models.OrderDetails> OrderDetails { get; set; }

        public DbSet<WoM_Labb1.Models.Order> Order { get; set; }
    }
}
