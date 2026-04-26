# GoodBurger - API

REST API for managing orders at the Good Burger snack bar.

> This application is also available through a pre-configured demo test environment at:  
> **http://161.153.109.111:5001**



[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4)](https://dotnet.microsoft.com/apps/aspnet)
[![SQLite](https://img.shields.io/badge/SQLite-3-003B57)](https://www.sqlite.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
---

![img.png](GoodBurger.Api/DOcs/GoodBurger-Demo.png)


---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Endpoints](#endpoints)
- [Discount Rules](#discount-rules)
- [Authentication](#authentication)
- [Pagination](#pagination)
- [Request & Response Examples](#request--response-examples)
- [Running Tests](#running-tests)
- [Running with Docker](#running-with-docker) 
- [Architecture Decisions](#architecture-decisions)
- [What Was Left Out](#what-was-left-out)

---

## Overview

This project is a technical challenge for the **Good Burger** snack bar. It provides a REST API built with ASP.NET Core to manage customer orders, apply discount rules automatically, and expose the full menu.

---

## Features

- Full CRUD for orders (Create, Read, Update, Delete)
- Automatic discount calculation based on combo rules
- Menu endpoint with prices fetched from the database
- Input validation with clear error messages
- Swagger UI for interactive documentation and testing
- SQLite database with persistent storage

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?logo=dotnet&logoColor=white) |
| Language | ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white) |
| Database | ![SQLite](https://img.shields.io/badge/SQLite-003B57?logo=sqlite&logoColor=white) |
| ORM | ![Entity Framework](https://img.shields.io/badge/EF_Core-512BD4?logo=dotnet&logoColor=white) |
| Documentation | ![Swagger](https://img.shields.io/badge/Swagger-85EA2D?logo=swagger&logoColor=black) |
| IDE | ![Rider](https://img.shields.io/badge/Rider-000000?logo=jetbrains&logoColor=white) |

---

## Project Structure

```
GoodBurger.Api/
├── Controllers/
│   ├── MenuController.cs         # GET /api/menu
│   └── OrdersController.cs       # CRUD /api/orders
├── Data/
│   └── AppDbContext.cs           # EF Core database context
├── Domain/
│   ├── Enums/
│   │   ├── SandwichType.cs       # XBurger, XEgg, XBacon
│   │   └── SideType.cs           # Fries, Soda
│   └── Models/
│       ├── MenuItem.cs           # Menu item entity
│       └── Order.cs              # Order entity
├── DTOs/
│   ├── CreateOrderRequest.cs     # POST body
│   ├── UpdateOrderRequest.cs     # PUT body
│   ├── OrderResponse.cs          # Order response shape
│   └── MenuItemResponse.cs       # Menu item response shape
├── Exceptions/
│   ├── OrderNotFoundException.cs   # 404
│   ├── DuplicateItemException.cs   # 409
│   └── InvalidOrderException.cs    # 400
├── Repositories/
│   ├── IOrderRepository.cs         # Repository contract
│   └── SqliteOrderRepository.cs    # EF Core implementation
├── Services/
│   ├── IOrderService.cs            # Service contract
│   ├── OrderService.cs             # Business logic + discount
│   ├── MenuService.cs              # Menu queries
│   └── DiscountCalculator.cs       # Discount rules
├── Program.cs                      # Entry point, DI, middleware
├── goodburger.db                   # SQLite database file
└── README.md                       # It's me!
```

---

## Getting Started

### Prerequisites

#### Operating System
- Linux, Windows or macOS

#### Runtime / SDK
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

#### Database (optional)
- Any SQLite viewer e.g. [DB Browser for SQLite](https://sqlitebrowser.org/)

#### IDE / Editor (optional)
- JetBrains Rider
- Visual Studio
- Visual Studio Code
- Any editor with C# support (like Notepad++ or VIM)
### Installation

```bash
# Clone the repository
git clone https://github.com/FlaipyTheHost/GoodBurger
cd GoodBurger

# Restore dependencies
dotnet restore

# Run the API
dotnet run
```

The API will start at `http://localhost:5290`.

### Swagger UI

Open your browser at:

```
http://localhost:5290/swagger
```

### Database Setup

The SQLite database file (`goodburger.db`) is created automatically on first run via `EnsureCreated()`.

Menu items are also **seeded automatically** during application startup, so no manual action is required, using the `AppDbContext.cs` class.

However, if you prefer to seed the data manually, you can run the following SQL directly on the `goodburger.db` file:

```sql
CREATE TABLE IF NOT EXISTS MenuItems (
                                         Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                                         Name     TEXT    NOT NULL,
                                         Price    REAL    NOT NULL,
                                         Category TEXT    NOT NULL
);

INSERT INTO MenuItems (Name, Price, Category) VALUES ('XBurger', 5.00, 'Sandwich');
INSERT INTO MenuItems (Name, Price, Category) VALUES ('XEgg',    4.50, 'Sandwich');
INSERT INTO MenuItems (Name, Price, Category) VALUES ('XBacon',  7.00, 'Sandwich');
INSERT INTO MenuItems (Name, Price, Category) VALUES ('Fries',   2.00, 'Side');
INSERT INTO MenuItems (Name, Price, Category) VALUES ('Soda',    2.50, 'Side');
```
---

## Endpoints

| Method | Route              | Description |
|---|--------------------|---|
| `POST` | `/api/Auth/login`   | Returns a JWT token for valid credentials. |
| `GET` | `/api/menu`        | Returns all menu items with prices |
| `GET` | `/api/orders`      | Returns paginated orders (supports `page` and `pageSize` query params) |
| `GET` | `/api/orders/{id}` | Returns a single order by ID |
| `POST` | `/api/orders`      | Creates a new order |
| `PUT` | `/api/orders/{id}` | Updates an existing order |
| `DELETE` | `/api/orders/{id}` | Deletes an order |

---

## Discount Rules

| Combo | Discount |
|---|---|
| Sandwich + Fries + Soda | **20% off** |
| Sandwich + Soda | **15% off** |
| Sandwich + Fries | **10% off** |
| Sandwich only | No discount |

Discounts are applied automatically on every `POST` and `PUT` request.

---

## Authentication

All endpoints except `POST /api/auth/login` require a valid JWT token.

### How to authenticate

**1. Generate a token:**

`POST /api/auth/login`

```json
{
  "username": "demo",
  "password": "demo123"
}
```

**Response 200 OK:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**2. Use the token in every request:**

Add the following header to all requests:

Ex: `Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

**3. Using Swagger UI:**

- Click the **Authorize** button at the top of the Swagger page
- Paste the token value and click **Authorize**
- All subsequent requests will include the token automatically

> Tokens expire after **8 hours**. Generate a new one via `POST /api/auth/login`.

> Default credentials are defined in `appsettings.json` under the `Auth` section.

---

## Pagination

The `GET /api/orders` endpoint supports pagination to efficiently handle large datasets.

### Query Parameters

| Parameter | Type | Default | Validation | Description |
|---|---|---|---|---|
| `page` | `int` | `1` | Must be ≥ 1 | The page number to retrieve |
| `pageSize` | `int` | `10` | Must be between 1 and 100 | Number of items per page |

### Response Format

The endpoint returns a `PagedResponse<OrderResponse>` object:

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "sandwich": "XBurger",
      "hasFries": true,
      "hasSoda": true,
      "subtotal": 9.50,
      "discountPercent": 20,
      "discount": 1.90,
      "total": 7.60,
      "createdAt": "2025-04-22T19:00:00Z",
      "updatedAt": "2025-04-22T19:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 42,
  "totalPages": 5
}
```

### Response Fields

| Field | Type | Description |
|---|---|---|
| `items` | `OrderResponse[]` | Array of orders for the current page |
| `page` | `int` | Current page number |
| `pageSize` | `int` | Number of items per page |
| `totalCount` | `int` | Total number of orders in the database |
| `totalPages` | `int` | Total number of pages available |

### Examples

**Get first page with default size (10 items):**
```
GET /api/orders
GET /api/orders?page=1
```

**Get second page with 20 items:**
```
GET /api/orders?page=2&pageSize=20
```

**Get all orders with maximum page size:**
```
GET /api/orders?pageSize=100
```

### Error Responses

**Invalid page number:**
```json
{
  "error": "Page must be greater than 0."
}
```

**Invalid page size:**
```json
{
  "error": "PageSize must be greater than 0."
}
```

**Page size too large:**
```json
{
  "error": "PageSize cannot exceed 100."
}
```

---

## Request & Response Examples

### POST /api/orders Create an order

**Request body:**
```json
{
  "sandwich": 0,
  "hasFries": true,
  "hasSoda": true
}
```

Sandwich values:

| Value | Item | Price |
|---|---|---|
| `0` | XBurger | R$ 5.00 |
| `1` | XEgg | R$ 4.50 |
| `2` | XBacon | R$ 7.00 |

**Response 201 Created:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sandwich": "XBurger",
  "hasFries": true,
  "hasSoda": true,
  "subtotal": 9.50,
  "discountPercent": 20,
  "discount": 1.90,
  "total": 7.60,
  "createdAt": "2025-04-22T19:00:00Z",
  "updatedAt": "2025-04-22T19:00:00Z"
}
```

### GET /api/orders/{id} Order not found

**Response 404 Not Found:**
```json
{
  "error": "Order with id '3fa85f64-5717-4562-b3fc-2c963f66afa6' was not found."
}
```

### POST /api/orders Invalid sandwich

**Response 400 Bad Request:**
```json
{
  "error": "Sandwich value '99' is not valid. Use 0 (XBurger), 1 (XEgg) or 2 (XBacon)."
}
```

---

## Running Tests

![img.png](GoodBurger.Api/DOcs/img.png)

The project includes comprehensive unit and integration tests for all major components.

### Test Structure

```
GoodBurger.Tests/
├── Services/
│   └── UnitTest1.cs
│       ├── DiscountCalculatorTests    # Unit tests for discount logic
│       └── OrderServiceTests          # Integration tests with in memory database
```

### Running All Tests

```bash
# From the project root directory
dotnet test

# Or specify the test project
dotnet test GoodBurger.Tests
```

### Running Tests with Detailed Output

```bash
# Show detailed test results
dotnet test --verbosity normal

# Show detailed output with individual test names
dotnet test --logger "console;verbosity=detailed"
```

### Running Specific Tests

```bash
# Run only DiscountCalculator tests
dotnet test --filter "FullyQualifiedName~DiscountCalculatorTests"

# Run only OrderService tests
dotnet test --filter "FullyQualifiedName~OrderServiceTests"

# Run a specific test by name
dotnet test --filter "FullyQualifiedName~Create_WithXBurgerFriesAndSoda_CreatesOrderWith20PercentDiscount"
```

### Test Coverage

The test suite includes:

**DiscountCalculator (4 tests):**
- 20% discount for sandwich + fries + soda
- 15% discount for sandwich + soda only
- 10% discount for sandwich + fries only
- 0% discount for sandwich only

**OrderService (16 integration tests):**

*Create Operations (5 tests):*
- Create order with all combos and correct discount calculation
- Verify order persistence in database

*Query Operations (5 tests):*
- Get order by ID (existing and non-existing)
- Get all orders with pagination
- Empty list when no orders exist

*Update Operations (3 tests):*
- Update existing order successfully
- Recalculate discounts on update
- Throw exception for non-existing order

*Delete Operations (3 tests):*
- Delete existing order successfully
- Throw exception for non-existing order
- Delete only specific order (verify isolation)

### Expected Output

When all tests pass, you should see:

```
Passed!  - Failed:     0, Passed:    20, Skipped:     0, Total:    20
```

### Testing Framework

The tests use:
- **xUnit** - Testing framework
- **FluentAssertions** - Assertion library for readable test code
- **SQLite In-Memory Database** - Isolated database for each test run
- **Entity Framework Core** - Real database operations without mocks

### Notes

- All tests use an in memory SQLite database, so they don't affect the production `goodburger.db` file
- Each test class disposes of its database context after running, ensuring test isolation
- No mocks are used for OrderService tests - they use real database operations for true integration testing

---

## Running with Docker

>This project can be run entirely with Docker, ensuring a consistent environment and avoiding "works on my machine" issues.

### Prerequisites

- Docker
- Docker Compose

---

###  Run the application

From the project root:

```
docker compose up --build
````

---

### Access

| Service   | URL                                                            |
| --------- | -------------------------------------------------------------- |
| Blazor UI | [http://localhost:5001](http://localhost:5001)                 |
| API       | [http://localhost:5290](http://localhost:5290)                 |
| Swagger   | [http://localhost:5290/swagger](http://localhost:5290/swagger) |

---

### How it works

* The frontend (Blazor) communicates with the API using Docker's internal network:

  ```
  http://api:8080
  ```

* Environment-based configuration is used:

    * Local → `localhost`
    * Docker → `api`

---

### Database

* SQLite database is persisted using a Docker volume
* Data is not lost when containers are recreated

---

### Case of Stop

```bash
docker compose down
```

## Architecture Decisions

### Layered architecture

The project follows a layered architecture where each layer only depends on the layer below it:

```
Controllers -> Services -> Repositories -> Domain
```

This makes it easy to swap implementations without touching business logic. For example, replacing SQLite with SQL Server only requires a new `IOrderRepository` implementation, nothing else changes.

### Entity Framework Core with SQLite

SQLite was chosen to keep the project self-contained with no external database server required. EF Core manages all SQL generation. Swapping to SQL Server or PostgreSQL would only require changing the connection string and the provider package.

### Repository pattern with interface

`IOrderRepository` decouples the service layer from the data layer. The service never knows whether data comes from SQLite, SQL Server, or an in-memory store.

### Menu prices in the database

Prices are stored in a `MenuItems` table instead of hardcoded values. This means prices can be updated directly in the database without recompiling the application.

### Global exception handler

All HTTP error mapping is centralized in `Program.cs`. Controllers and services only throw typed exceptions (`OrderNotFoundException`, `InvalidOrderException`). The handler maps each type to the correct HTTP status code, keeping the rest of the code clean.

### DTOs separate from domain models

Request and response shapes are defined as separate `record` types in the `DTOs` folder. This allows the internal domain model to evolve independently from the API contract.

---

## What Was Left Out

The following items were considered out of scope for this challenge:

- **Migrations** : `EnsureCreated()` is used instead of EF Core migrations, which is suitable for development but not recommended for production
- **HTTPS** : disabled for local development simplicity

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
