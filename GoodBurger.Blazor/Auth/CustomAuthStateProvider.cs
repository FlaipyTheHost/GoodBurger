using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace GoodBurger.Blazor.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenStorage _tokenStorage;

    public CustomAuthStateProvider(TokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();

        var identity = string.IsNullOrWhiteSpace(token)
            ? new ClaimsIdentity()
            : new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "admin") }, "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task NotifyUserAuthentication(string token)
    {
        await _tokenStorage.SetTokenAsync(token);

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "admin") }, "jwt");

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public async Task NotifyUserLogout()
    {
        await _tokenStorage.ClearAsync();

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }
}