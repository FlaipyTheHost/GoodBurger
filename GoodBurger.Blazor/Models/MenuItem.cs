namespace GoodBurger.Blazor.Models;

public record MenuItem(
    int Id,
    string Name,
    decimal Price,
    string Category
);
