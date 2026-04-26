using System.Net.Http.Headers;
using System.Net.Http.Json;
using GoodBurger.Blazor.Models;
using GoodBurger.Blazor.Auth;

namespace GoodBurger.Blazor.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        CustomAuthStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login",
                new LoginRequest(username, password));

            if (!response.IsSuccessStatusCode) return false;

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse?.Token is null) return false;

            await _authStateProvider.NotifyUserAuthentication(loginResponse.Token);
            SetAuthorizationHeader(loginResponse.Token);

            return true;
        }
        catch { return false; }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }
}