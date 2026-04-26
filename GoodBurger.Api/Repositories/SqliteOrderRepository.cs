using GoodBurger.Data;
using GoodBurger.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodBurger.Repositories;

public class SqliteOrderRepository(AppDbContext context) : IOrderRepository
{
    public (IEnumerable<Order> Items, int TotalCount) GetAll(int page, int pageSize)
    {
        var totalCount = context.Orders.Count();
        var items = context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return (items, totalCount);
    }

    public Order? GetById(Guid id) =>
        context.Orders.FirstOrDefault(o => o.Id == id);

    public Order Add(Order order)
    {
        context.Orders.Add(order);
        context.SaveChanges();
        return order;
    }

    public Order Update(Order order)
    {
        context.Orders.Update(order);
        context.SaveChanges();
        return order;
    }

    public bool Delete(Guid id)
    {
        var order = context.Orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return false;
        context.Orders.Remove(order);
        context.SaveChanges();
        return true;
    }
}
