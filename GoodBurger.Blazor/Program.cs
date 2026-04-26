using GoodBurger.Blazor.Components;
using GoodBurger.Blazor.Services;
using GoodBurger.Blazor.Auth;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // Add services to the container.
 
builder.Services.AddScoped<TokenStorage>(); // Register TokenStorage as scoped (per users)

 
builder.Services.AddAuthorizationCore(); //-- AuthorizationProvider

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    provider => provider.GetRequiredService<CustomAuthStateProvider>()
);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                 ?? throw new Exception("ApiBaseUrl not settings"); // Read configs


// ----    Configure HttpClient for Auth -
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});


builder.Services.AddHttpClient<GoodBurgerApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl); // --Configure HttpClient for API
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();