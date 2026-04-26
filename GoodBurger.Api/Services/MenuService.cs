using GoodBurger.Data;
using GoodBurger.Domain.Enums;
using GoodBurger.Domain.Models;

namespace GoodBurger.Services;

public class MenuService(AppDbContext context)
{
    public IEnumerable<MenuItem> GetMenu() =>
        context.MenuItems.ToList();

    public decimal GetPrice(string name) =>
        context.MenuItems.First(m => m.Name == name).Price;

    public Dictionary<SandwichType, decimal> GetSandwichPrices() =>
        context.MenuItems
            .Where(m => m.Category == "Sandwich")
            .AsEnumerable()
            .ToDictionary(
                m => Enum.Parse<SandwichType>(m.Name),
                m => m.Price
            );

    public decimal GetFriesPrice() =>
        context.MenuItems.First(m => m.Name == "Fries").Price;

    public decimal GetSodaPrice() =>
        context.MenuItems.First(m => m.Name == "Soda").Price;
}
