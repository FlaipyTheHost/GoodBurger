using System.Net.Http.Headers;
using System.Net.Http.Json;
using GoodBurger.Blazor.Auth;
using GoodBurger.Blazor.Models;

namespace GoodBurger.Blazor.Services;

public class GoodBurgerApiService
{
    private readonly HttpClient _httpClient;
    private readonly TokenStorage _tokenStorage;

    public GoodBurgerApiService(HttpClient httpClient, TokenStorage tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<MenuItem>?> GetMenuAsync()
    {
        await SetAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<List<MenuItem>>("/api/Menu");
    }

    public async Task<PagedResponse<OrderResponse>?> GetOrdersAsync(int page = 1, int pageSize = 100)
    {
        await SetAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<PagedResponse<OrderResponse>>($"/api/Orders?page={page}&pageSize={pageSize}");
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(Guid id)
    {
        await SetAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<OrderResponse>($"/api/Orders/{id}");
    }

    public async Task<OrderResponse?> CreateOrderAsync(CreateOrderRequest request)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/api/Orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderResponse>();
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.DeleteAsync($"/api/Orders/{id}");
        response.EnsureSuccessStatusCode();
    }
}