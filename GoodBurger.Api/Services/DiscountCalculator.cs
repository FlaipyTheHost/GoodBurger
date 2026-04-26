namespace GoodBurger.Services;

public class DiscountCalculator
{
    public decimal GetDiscountPercent(bool hasFries, bool hasSoda) =>
        (hasFries, hasSoda) switch
        {
            (true, true)  => 20m,
            (false, true) => 15m,
            (true, false) => 10m,
            _             => 0m
        };
}
