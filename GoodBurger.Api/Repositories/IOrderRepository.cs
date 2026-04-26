using GoodBurger.Domain.Models;

namespace GoodBurger.Repositories;

public interface IOrderRepository
{
    (IEnumerable<Order> Items, int TotalCount) GetAll(int page, int pageSize);
    Order? GetById(Guid id);
    Order Add(Order order);
    Order Update(Order order);
    bool Delete(Guid id);
}
