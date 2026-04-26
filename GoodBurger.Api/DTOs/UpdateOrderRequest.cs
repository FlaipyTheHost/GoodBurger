using GoodBurger.Domain.Enums;

namespace GoodBurger.DTOs;

public record UpdateOrderRequest(
    SandwichType Sandwich,
    bool HasFries,
    bool HasSoda
);
