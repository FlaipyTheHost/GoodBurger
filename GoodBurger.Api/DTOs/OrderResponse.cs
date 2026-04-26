namespace GoodBurger.DTOs;

public record OrderResponse(
    Guid Id,
    string Sandwich,
    bool HasFries,
    bool HasSoda,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal Discount,
    decimal Total,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
