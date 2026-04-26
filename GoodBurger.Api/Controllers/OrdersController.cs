using GoodBurger.DTOs;
using GoodBurger.Exceptions;
using GoodBurger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodBurger.Controllers;

/// <summary>Manages orders for the Good Burger snack bar.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>Returns a paginated list of orders.</summary>
    /// <param name="page">Page number, starting at 1.</param>
    /// <param name="pageSize">Number of items por page. Default is 10.</param>
    [HttpGet]
    public ActionResult<PagedResponse<OrderResponse>> GetAll(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1)     return BadRequest(new { error = "Page must be greater than 0." });
        if (pageSize < 1) return BadRequest(new { error = "PageSize must be greater than 0." });
        if (pageSize > 100) return BadRequest(new { error = "PageSize cannot exceed 100." });

        return Ok(orderService.GetAll(page, pageSize));
    }

    /// <summary>Returns a single order by ID.</summary>
    /// <param name="id">The order ID.</param>
    [HttpGet("{id:guid}")]
    public ActionResult<OrderResponse> GetById(Guid id)
    {
        try { return Ok(orderService.GetById(id)); }
        catch (OrderNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    /// <summary>Creates a new order and calculates discounts automatically.</summary>
    /// <remarks>
    /// Discount rules
    /// Sandwich + Fries + Soda = 20% off
    /// Sandwich + Soda = 15% off
    /// Sandwich + Fries = 10% off
    /// Sandwich only = no discount
    ///
    /// Sandwich values: 0 = XBurger R$ 5.00, 1 = XEgg 4.50 reais, 2 = XBacon R$7.00
    /// </remarks>
    [HttpPost]
    public ActionResult<OrderResponse> Create([FromBody] CreateOrderRequest request)
    {
        var response = orderService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>Updates an existing order.</summary>
    /// <param name="id">The order GUID.</param>
    [HttpPut("{id:guid}")]
    public ActionResult<OrderResponse> Update(Guid id, [FromBody] UpdateOrderRequest request)
    {
        try { return Ok(orderService.Update(id, request)); }
        catch (OrderNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    /// <summary>Deletes an order by ID.</summary>
    /// <param name="id">The order ID.</param>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try { orderService.Delete(id); return NoContent(); }
        catch (OrderNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }
}
