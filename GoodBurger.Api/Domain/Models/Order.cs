using GoodBurger.Domain.Enums;

namespace GoodBurger.Domain.Models;

public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public SandwichType Sandwich { get; set; }
    public bool HasFries { get; set; }
    public bool HasSoda { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
