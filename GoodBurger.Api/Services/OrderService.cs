using GoodBurger.Domain.Models;
using GoodBurger.DTOs;
using GoodBurger.Exceptions;
using GoodBurger.Repositories;

namespace GoodBurger.Services;

public interface IOrderService
{
    PagedResponse<OrderResponse> GetAll(int page, int pageSize);
    OrderResponse GetById(Guid id);
    OrderResponse Create(CreateOrderRequest request);
    OrderResponse Update(Guid id, UpdateOrderRequest request);
    void Delete(Guid id);
}

public class OrderService(
    IOrderRepository repository,
    MenuService menuService) : IOrderService
{
    public PagedResponse<OrderResponse> GetAll(int page, int pageSize)
    {
        var (items, totalCount) = repository.GetAll(page, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResponse<OrderResponse>(
            Items:      items.Select(ToResponse),
            Page:       page,
            PageSize:   pageSize,
            TotalCount: totalCount,
            TotalPages: totalPages
        );
    }

    public OrderResponse GetById(Guid id)
    {
        var order = repository.GetById(id) ?? throw new OrderNotFoundException(id);
        return ToResponse(order);
    }

    public OrderResponse Create(CreateOrderRequest request)
    {
        Validate(request.Sandwich);
        var order = BuildOrder(new Order(), request.Sandwich, request.HasFries, request.HasSoda);
        return ToResponse(repository.Add(order));
    }

    public OrderResponse Update(Guid id, UpdateOrderRequest request)
    {
        var existing = repository.GetById(id) ?? throw new OrderNotFoundException(id);
        Validate(request.Sandwich);
        var updated = BuildOrder(existing, request.Sandwich, request.HasFries, request.HasSoda);
        updated.UpdatedAt = DateTime.UtcNow;
        return ToResponse(repository.Update(updated));
    }

    public void Delete(Guid id)
    {
        if (!repository.Delete(id))
            throw new OrderNotFoundException(id);
    }

    // ---validation begin here--

    private static void Validate(GoodBurger.Domain.Enums.SandwichType sandwich)
    {
        if (!Enum.IsDefined(sandwich))
            throw new InvalidOrderException(
                $"Sandwich value '{(int)sandwich}' is not valid. Use 0 (XBurger), 1 (XEgg) or 2 (XBacon).");
    }

    // ---- helpers--

    private Order BuildOrder(Order order, GoodBurger.Domain.Enums.SandwichType sandwich, bool hasFries, bool hasSoda)
    {
        var sandwichPrices = menuService.GetSandwichPrices();
        var friesPrice     = menuService.GetFriesPrice();
        var sodaPrice      = menuService.GetSodaPrice();

        order.Sandwich = sandwich;
        order.HasFries = hasFries;
        order.HasSoda  = hasSoda;

        var sandwichPrice = sandwichPrices[sandwich];
        var sidesTotal    = (hasFries ? friesPrice : 0) + (hasSoda ? sodaPrice : 0);

        order.Subtotal        = sandwichPrice + sidesTotal;
        order.DiscountPercent = new DiscountCalculator().GetDiscountPercent(hasFries, hasSoda);
        order.Discount        = Math.Round(order.Subtotal * order.DiscountPercent / 100, 2);
        order.Total           = order.Subtotal - order.Discount;

        return order;
    }

    private static OrderResponse ToResponse(Order o) => new(
        o.Id, o.Sandwich.ToString(), o.HasFries, o.HasSoda,
        o.Subtotal, o.DiscountPercent, o.Discount, o.Total,
        o.CreatedAt, o.UpdatedAt
    );
}
