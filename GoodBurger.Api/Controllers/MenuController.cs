using GoodBurger.DTOs;
using GoodBurger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodBurger.Controllers;

/// <summary>Exposes the Good Burger menu.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenuController(MenuService menuService) : ControllerBase
{
    /// <summary>Returns all available menu itens width their prices.</summary>
    [HttpGet]
    public ActionResult<IEnumerable<MenuItemResponse>> GetMenu()
    {
        var items = menuService.GetMenu().Select(i => new MenuItemResponse(i.Name, i.Price, i.Category));
        return Ok(items);
    }
}
