using GoodBurger.Auth;
using Microsoft.AspNetCore.Mvc;

namespace GoodBurger.Controllers;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token);

/// <summary>Handles authentication.</summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenService tokenService, IConfiguration configuration) : ControllerBase
{
    /// <summary>Returns a JWT token for valid credentials.</summary>
    /// <remarks>
    /// Deafult credentials for testing are useing:
    /// user: demo
    /// pass: demo123
    /// </remarks>
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var validUser     = configuration["Auth:Username"];
        var validPassword = configuration["Auth:Password"];

        if (request.Username != validUser || request.Password != validPassword)
            return Unauthorized(new { error = "Invalid username or password." });

        var token = tokenService.GenerateToken(request.Username);
        return Ok(new LoginResponse(token));
    }
}
