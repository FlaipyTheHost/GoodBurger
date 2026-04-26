namespace GoodBurger.Blazor.Models;

public record CreateOrderRequest(
    int Sandwich,
    bool HasFries,
    bool HasSoda
);
