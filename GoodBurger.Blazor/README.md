# GoodBurger - Blazor Web App
 
> Blazor Server frontend for the Good Burger order management system.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![MaterializeCSS](https://img.shields.io/badge/MaterializeCSS-1.0-EE6E73)](https://materializecss.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Pages](#pages)
- [Authentication](#authentication)
- [Discount Rules](#discount-rules)
- [Session Behavior](#session-behavior)
- [Architecture Decisions](#architecture-decisions)
- [What Was Left Out](#what-was-left-out)

---

## Overview

This project is the optional frontend differential for the **Good Burger** technical challenge. It provides a Blazor Server web interface to interact with the Good Burger REST API, allowing users to log in, create orders, view order history, and see automatic discount calculations in real time.

---

## Features

- **Authentication** — JWT token-based login and logout
- **Session Persistence** — Token stored in `ProtectedSessionStorage` (persists on page reload, isolated per browser tab)
- **Multi-user Support** — Each browser session is fully isolated; multiple users can be logged in simultaneously
- **Home Page** — Welcome page with quick access to main features and combo discount overview
- **New Order** — Interactive form with real-time price and discount calculation
- **Order List** — All orders sorted by date with full details and delete functionality
- **Protected Routes** — Pages require authentication via `<AuthorizeView>`
- **Clean UI** — Built with MaterializeCSS for a responsive, modern look

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ![Blazor](https://img.shields.io/badge/Blazor_Server-512BD4?logo=dotnet&logoColor=white) |
| Language | ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white) |
| UI | ![MaterializeCSS](https://img.shields.io/badge/MaterializeCSS-EE6E73?logo=google&logoColor=white) |
| Auth Storage | ![ASP.NET](https://img.shields.io/badge/ProtectedSessionStorage-512BD4?logo=dotnet&logoColor=white) |
| HTTP Client | ![.NET](https://img.shields.io/badge/HttpClient-512BD4?logo=dotnet&logoColor=white) |
| IDE | ![Rider](https://img.shields.io/badge/Rider-000000?logo=jetbrains&logoColor=white) |

---

## Project Structure

```
GoodBurger.Blazor/
├── Auth/
│   ├── CustomAuthStateProvider.cs  # Blazor auth state provider (async, session-aware)
│   └── TokenStorage.cs             # Token storage via ProtectedSessionStorage
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor        # Main layout with navbar and footer
│   │   └── NavBar.razor            # Navbar with auth-aware menu (AuthorizeView)
│   ├── Pages/
│   │   ├── Home.razor              # Welcome page
│   │   ├── Login.razor             # Login page
│   │   ├── NewOrder.razor          # Create new order (protected)
│   │   ├── Orders.razor            # Order list (protected)
│   │   └── NotFound.razor          # 404 page
│   ├── App.razor                   # Root component with CascadingAuthenticationState
│   └── Routes.razor                # Router with AuthorizeRouteView + InteractiveServer
├── Models/
│   ├── CreateOrderRequest.cs       # DTO for creating orders
│   ├── OrderResponse.cs            # DTO for order response
│   ├── MenuItem.cs                 # DTO for menu items
│   ├── PagedResponse.cs            # DTO for paginated responses
│   ├── LoginRequest.cs             # DTO for login
│   └── LoginResponse.cs            # DTO for login response with token
├── Services/
│   ├── GoodBurgerApiService.cs     # API communication (injects auth header per request)
│   └── AuthService.cs              # Authentication service (login/logout)
├── Program.cs                      # App configuration and DI registration
└── appsettings.json                # Configuration (API base URL)
```

---

## Getting Started

### Prerequisites

#### Operating System
- Linux, Windows or macOS

#### Runtime / SDK
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

#### Required
- Good Burger API running on `http://localhost:5290`

#### IDE / Editor (optional)
- JetBrains Rider
- Visual Studio
- Visual Studio Code

### Installation

```bash
# Clone the repository
git clone https://github.com/FlaipyTheHost/GoodBurger
cd GoodBurger

# Restore dependencies
dotnet restore

# Start the API first
cd GoodBurger.Api
dotnet run

# Then start the Blazor app in a new terminal
cd GoodBurger.Blazor
dotnet run
```

The Blazor app will start at the URL shown in the terminal (usually `http://localhost:5272`).

### Login

Use the default credentials defined in the API's `appsettings.json`:

| Field | Value     |
|---|-----------|
| Username | `demo`    |
| Password | `demo123` |

---

## Pages

### Home (`/`)
- Welcome page with feature overview and combo discount cards
- Unauthenticated users see a login prompt via `<AuthorizeView>`
- Quick access buttons to **New Order** and **Orders**

### Login (`/login`)
- Username and password form
- On success: token saved to `ProtectedSessionStorage`, redirect to home
- On failure: inline error message

### New Order (`/new-order`) — Protected
- Select sandwich from the live menu fetched from the API
- Toggle optional fries and soda
- Real-time order summary showing:
    - Subtotal
    - Discount percentage and amount (if applicable)
    - Final total
- On confirm: order sent to API, redirect to order list after 2 seconds

### Orders (`/orders`) — Protected
- All orders sorted by date (newest first)
- Each order card shows: ID, date/time, sandwich, fries, soda, subtotal, discount, and total
- Delete order with browser confirmation dialog
- Button to create a new order

### Not Found (`/not-found`)
- Friendly 404 page for unknown routes

---

## Authentication

The app uses JWT token authentication integrated with Blazor's built-in auth infrastructure.

### Login Flow

```
User submits credentials
  → POST /api/auth/login
  → API validates against appsettings.json
  → Returns JWT token
  → Token saved to ProtectedSessionStorage
  → CustomAuthStateProvider notifies Blazor
  → UI updates reactively (NavBar, AuthorizeView)
```

### Token Injection

Every API call in `GoodBurgerApiService` calls `TokenStorage.GetTokenAsync()` before sending the request and sets the `Authorization: Bearer <token>` header directly on `HttpClient.DefaultRequestHeaders`.

### Protected Routes

All protected pages use `<AuthorizeView>` with `<Authorized>` and `<NotAuthorized>` blocks:

```razor
<AuthorizeView>
    <Authorized>
        <!-- Page content -->
    </Authorized>
    <NotAuthorized>
        <!-- Login prompt -->
    </NotAuthorized>
</AuthorizeView>
```

`Routes.razor` uses `<AuthorizeRouteView>` with `@rendermode InteractiveServer` applied globally, and `App.razor` wraps everything in `<CascadingAuthenticationState>` to propagate auth state across the component tree.

### Logout Flow

```
User clicks Logout
  → AuthService.LogoutAsync()
  → Token deleted from ProtectedSessionStorage
  → CustomAuthStateProvider notifies Blazor
  → UI updates reactively
  → Redirect to /login
```

### Default Credentials

Credentials are defined in the API's `appsettings.json` under the `Auth` key:

```json
"Auth": {
  "Username": "demo",
  "Password": "demo123"
}
```

> Tokens expire after **8 hours**. The user will need to log in again after expiry.

---

## Discount Rules

Discounts are calculated live on the **New Order** page as the user selects items, mirroring the API's discount logic:

| Combo | Discount |
|---|---|
| Sandwich + Fries + Soda | **20% off** |
| Sandwich + Soda | **15% off** |
| Sandwich + Fries | **10% off** |
| Sandwich only | No discount |

---

## Session Behavior

| Scenario | Behavior |
|---|---|
| Page reload (same tab) | Stays logged in |
| New tab or new window | Requires a new login (isolated session) |
| Multiple browsers simultaneously | Each session fully independent |
| Logout | Clears token and redirects to `/login` |
| Token expired (8h) | API returns 401, user must log in again |

---

## Architecture Decisions

### ProtectedSessionStorage instead of in-memory token

The original implementation stored the JWT token in a plain C# field in memory, which caused two problems: the token was lost on every page reload, and registering `TokenStorage` as `Singleton` leaked tokens between users. `ProtectedSessionStorage` solves both — it persists across reloads within the same tab and is scoped per browser session, so each user has their own isolated token.

### AuthorizeView instead of manual IsAuthenticated checks

Pages previously used `AuthService.IsAuthenticated` directly in `@if` blocks. This approach breaks when auth state changes asynchronously (e.g. after logout). Replacing it with `<AuthorizeView>` delegates state management to Blazor's auth infrastructure, which reacts automatically to `NotifyAuthenticationStateChanged` events.

### Single rendermode at the router level

Setting `@rendermode InteractiveServer` on individual pages caused each component to create an isolated Blazor circuit, breaking the `CascadingAuthenticationState` cascade from `App.razor`. Moving the rendermode to `Routes.razor` applies it to the entire component tree at once, ensuring the auth state cascades correctly to all child components.

### AuthorizeRouteView instead of RouteView

Replacing `<RouteView>` with `<AuthorizeRouteView>` in `Routes.razor` properly integrates the router with the `CascadingAuthenticationState` provided by `App.razor`, eliminating the `InvalidOperationException` that occurred when `<AuthorizeView>` couldn't find the cascading `Task<AuthenticationState>` parameter.

### Token injection per request in GoodBurgerApiService

The original `AuthHeaderHandler` (`DelegatingHandler`) conflicted with Blazor Server's DI lifecycle — it is resolved as `Transient` while `TokenStorage` is `Scoped`, causing the handler to hold a stale or null token. Injecting `TokenStorage` directly into `GoodBurgerApiService` and calling `GetTokenAsync()` before each request ensures the correct token is always used within the active scope.

### Credentials in appsettings.json (no user database)

Since this is a portfolio and demonstration project with a single fixed user, storing credentials in `appsettings.json` is sufficient. The API validates the username and password directly from configuration, keeping the setup simple and self-contained with no user management overhead.

---

## What Was Left Out

The following items were considered out of scope:

- **Token expiry handling** — Expired tokens return 401 from the API but the UI does not automatically redirect to login; the user sees an error message instead
- **Order editing** — The `PUT /api/orders/{id}` endpoint exists on the API but is not exposed in the UI
- **Pagination controls** — The order list fetches up to 100 orders in a single request; no UI pagination was implemented
- **Toast notifications** — Feedback is shown via inline card panels rather than toast messages
- **Mobile hamburger menu** — The navbar collapses on small screens but has no mobile drawer
- **Search and filtering** — No order search or date filtering on the orders page