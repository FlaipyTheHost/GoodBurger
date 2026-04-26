namespace GoodBurger.DTOs;

public record MenuItemResponse(
    string Name,
    decimal Price,
    string Category
);
