using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace GoodBurger.Blazor.Auth;

public class TokenStorage
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private const string Key = "auth_token";

    public TokenStorage(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(Key);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        await _sessionStorage.SetAsync(Key, token);
    }

    public async Task ClearAsync()
    {
        await _sessionStorage.DeleteAsync(Key);
    }
}