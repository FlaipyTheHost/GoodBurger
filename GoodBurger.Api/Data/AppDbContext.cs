using GoodBurger.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodBurger.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "XBurger", Price = 5.00m, Category = "Sandwich" },
            new MenuItem { Id = 2, Name = "XEgg",    Price = 4.50m, Category = "Sandwich" },
            new MenuItem { Id = 3, Name = "XBacon",  Price = 7.00m, Category = "Sandwich" },
            new MenuItem { Id = 4, Name = "Fries",   Price = 2.00m, Category = "Side"     },
            new MenuItem { Id = 5, Name = "Soda",    Price = 2.50m, Category = "Side"     }
        );
    }
}
