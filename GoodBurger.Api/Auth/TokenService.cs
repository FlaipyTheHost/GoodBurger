using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GoodBurger.Auth;

public class TokenService(IConfiguration configuration)
{
    public string GenerateToken(string username)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var token = new JwtSecurityToken(
            issuer:   configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims:   [new Claim(ClaimTypes.Name, username)],
            expires:  expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
