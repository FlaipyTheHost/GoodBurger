using GoodBurger.Domain.Enums;

namespace GoodBurger.DTOs;

public record CreateOrderRequest(
    SandwichType Sandwich,
    bool HasFries,
    bool HasSoda
);
