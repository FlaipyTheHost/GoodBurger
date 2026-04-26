using FluentAssertions;
using GoodBurger.Services;
using GoodBurger.Domain.Enums;
using GoodBurger.DTOs;
using GoodBurger.Repositories;
using GoodBurger.Domain.Models;
using GoodBurger.Exceptions;
using GoodBurger.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodBurger.Tests.Services;

// -------------
// Unit Test DiscountCalculator
// -------
#region Discount
public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _calculator = new();

    [Fact]
    public void GetDiscountPercent_WithFriesAndSoda_Returns20Percent()
    {
        // -- Act
        var discount = _calculator.GetDiscountPercent(hasFries: true, hasSoda: true);

        // Assert
        discount.Should().Be(20m);
    }

    [Fact]
    public void GetDiscountPercent_OnlyWithSoda_Returns15Percent()
    {
        // Act
        var discount = _calculator.GetDiscountPercent(hasFries: false, hasSoda: true);

        // Assert --
        discount.Should().Be(15m);
    }

    [Fact]
    public void GetDiscountPercent_OnlyWithFries_Returns10Percent()
    {
        // Act
        var discount = _calculator.GetDiscountPercent(hasFries: true, hasSoda: false);

        // Assert
        discount.Should().Be(10m);
    }

    [Fact]
    public void GetDiscountPercent_WithoutAnything_ReturnsZero()
    {
        // Act --
        var discount = _calculator.GetDiscountPercent(hasFries: false, hasSoda: false);

        // Assert
        discount.Should().Be(0m);
    }
}
#endregion

// ------
// Integration Test: oRDERsERVICE
// ----
#region OS
public class OrderServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IOrderRepository _repository;
    private readonly MenuService _menuService;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        // Configuring sqlite in memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new SqliteOrderRepository(_context);
        _menuService = new MenuService(_context);
        _orderService = new OrderService(_repository, _menuService);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    // --Create Tests----

    [Fact]
    public void Create_WithXBurgerFriesAndSoda_CreatesOrderWith20PercentDiscount()
    {
        // Arrange
        var request = new CreateOrderRequest(SandwichType.XBurger, true, true);

        // Act
        var result = _orderService.Create(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Sandwich.Should().Be("XBurger");
        result.HasFries.Should().BeTrue();
        result.HasSoda.Should().BeTrue();
        result.Subtotal.Should().Be(9.5m); // 5.00 + 2.00 + 2.50
        result.DiscountPercent.Should().Be(20m);
        result.Discount.Should().Be(1.9m);
        result.Total.Should().Be(7.6m);
    }

    [Fact]
    public void Create_WithXEggOnlyFries_CreatesOrderWith10PercentDiscount()
    {
        // Arrange ---
        var request = new CreateOrderRequest(SandwichType.XEgg, true, false);

        // Act
        var result = _orderService.Create(request);

        //-- Assert --
        result.Sandwich.Should().Be("XEgg");
        result.HasFries.Should().BeTrue();
        result.HasSoda.Should().BeFalse();
        result.Subtotal.Should().Be(6.5m); // 4.50 + 2.00
        result.DiscountPercent.Should().Be(10m);
        result.Discount.Should().Be(0.65m);
        result.Total.Should().Be(5.85m);
        // -----
    }

    [Fact]
    public void Create_WithXBaconOnlySoda_CreatesOrderWith15PercentDiscount()
    {
        // Arrange
        var request = new CreateOrderRequest(SandwichType.XBacon, false, true);

        // Act
        var result = _orderService.Create(request);

        // Assert
        result.Sandwich.Should().Be("XBacon");
        result.HasFries.Should().BeFalse();
        result.HasSoda.Should().BeTrue();
        result.Subtotal.Should().Be(9.5m); // 7.00 + 2.50
        result.DiscountPercent.Should().Be(15m);
        result.Discount.Should().Be(1.42m);
        result.Total.Should().Be(8.08m);
        // -----

    }

    [Fact]
    public void Create_WithoutSides_CreatesOrderWithoutDiscount()
    {
        // Arrange
        var request = new CreateOrderRequest(SandwichType.XBurger, false, false);

        // Act
        var result = _orderService.Create(request);

        // Assert
        result.Subtotal.Should().Be(5m);
        result.DiscountPercent.Should().Be(0m);
        result.Discount.Should().Be(0m);
        result.Total.Should().Be(5m);
        // -----

    }

    [Fact]
    public void Create_InsertedOrder_ShouldPersistInDatabase()
    {
        // Arrange
        var request = new CreateOrderRequest(SandwichType.XBurger, true, true);

        // Act
        var created = _orderService.Create(request);

        // Assert --
        var fromDb = _context.Orders.Find(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Sandwich.Should().Be(SandwichType.XBurger);
        fromDb.Total.Should().Be(7.6m);
        // -----

    }

    // -- Query Tests -----

    [Fact]
    public void GetById_WhenOrderExists_ReturnsOrder()
    {
        // Arrange --- Create an order first
        var request = new CreateOrderRequest(SandwichType.XEgg, true, true);
        var created = _orderService.Create(request);

        // Act
        var result = _orderService.GetById(created.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(created.Id);
        result.Sandwich.Should().Be("XEgg");
        result.Total.Should().Be(created.Total);
        //-----
    }

    [Fact]
    public void GetById_WhenOrderDoesNotExist_ThrowsOrderNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var act = () => _orderService.GetById(id);

        // Assert
        act.Should().Throw<OrderNotFoundException>()
            .WithMessage($"Order with id '{id}' was not found.");
    }

    [Fact]
    public void GetAll_WithoutOrders_ReturnsEmptyList()
    {
        // Act
        var result = _orderService.GetAll(1, 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        //---
    }

    [Fact]
    public void GetAll_WithMultipleOrders_ReturnsAllOrders()
    {
        // Arrange - Create 3 orders
        _orderService.Create(new CreateOrderRequest(SandwichType.XBurger, true, true));
        _orderService.Create(new CreateOrderRequest(SandwichType.XEgg, false, true));
        _orderService.Create(new CreateOrderRequest(SandwichType.XBacon, true, false));
    //---
        
        // Act
        var result = _orderService.GetAll(1, 10);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public void GetAll_WithPagination_ReturnsCorrectPage()
    {
        // Arrange - Create 5 orders
        for (int i = 0; i < 5; i++)
            _orderService.Create(new CreateOrderRequest(SandwichType.XBurger, true, true));

        // Act - Request page 2 with 2 items per page
        var result = _orderService.GetAll(2, 2);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
    }

    // --- Updates!!! ---

    [Fact]
    public void Update_ExistingOrder_UpdatesSuccessfully()
    {
        // Arrange
        var created = _orderService.Create(new CreateOrderRequest(SandwichType.XBurger, false, false));
        var updateRequest = new UpdateOrderRequest(SandwichType.XBacon, true, true);

        // Act
        var updated = _orderService.Update(created.Id, updateRequest);

        // Assert
        updated.Id.Should().Be(created.Id);
        updated.Sandwich.Should().Be("XBacon");
        updated.HasFries.Should().BeTrue();
        updated.HasSoda.Should().BeTrue();
        updated.Subtotal.Should().Be(11.5m); // 7.00 + 2.00 + 2.50
        updated.DiscountPercent.Should().Be(20m);
        updated.Total.Should().Be(9.2m);
        //--
    }

    [Fact]
    public void Update_ExistingOrder_RecalculatesDiscounts()
    {
        // Arrange - Order without discount
        var created = _orderService.Create(new CreateOrderRequest(SandwichType.XBurger, false, false));
        created.DiscountPercent.Should().Be(0m);

        // Act - Update to have discount
        var updateRequest = new UpdateOrderRequest(SandwichType.XBurger, true, true);
        var updated = _orderService.Update(created.Id, updateRequest);

        // Assert - Should have recalculated the discount
        updated.DiscountPercent.Should().Be(20m);
        updated.Discount.Should().Be(1.9m);
    }

    [Fact]
    public void Update_NonExistingOrder_ThrowsOrderNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateOrderRequest(SandwichType.XBurger, true, true);

        // Act
        var act = () => _orderService.Update(id, request);

        // Assert
        act.Should().Throw<OrderNotFoundException>();
    }

    // --- dELETE tESTS ----

    [Fact]
    public void Delete_ExistingOrder_RemovesSuccessfully()
    {
        // Arrange
        var created = _orderService.Create(new CreateOrderRequest(SandwichType.XBurger, true, true));

        // Act
        _orderService.Delete(created.Id);

        // Assert - Verify it no longer exists
        var act = () => _orderService.GetById(created.Id);
        act.Should().Throw<OrderNotFoundException>();
    }

    [Fact]
    public void Delete_NonExistingOrder_ThrowsOrderNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var act = () => _orderService.Delete(id);

        // Assert
        act.Should().Throw<OrderNotFoundException>();
    }

    [Fact]
    public void Delete_ExistingOrder_RemovesOnlySpecificOrder()
    {
        // Arrange -- Create 3 orders
        var order1 = _orderService.Create(new CreateOrderRequest(SandwichType.XBurger, true, true));
        var order2 = _orderService.Create(new CreateOrderRequest(SandwichType.XEgg, false, true));
        var order3 = _orderService.Create(new CreateOrderRequest(SandwichType.XBacon, true, false));

        // Act --- Delete only order2
        _orderService.Delete(order2.Id);

        // Assert
        var remaining = _orderService.GetAll(1, 10);
        remaining.TotalCount.Should().Be(2);
        remaining.Items.Should().Contain(o => o.Id == order1.Id);
        remaining.Items.Should().Contain(o => o.Id == order3.Id);
        remaining.Items.Should().NotContain(o => o.Id == order2.Id);
    }
}
#endregion
