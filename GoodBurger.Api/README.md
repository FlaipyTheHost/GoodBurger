# GoodBurger - API

REST API for managing orders at the Good Burger snack bar.

> This application is also available through a pre-configured demo test environment at:  
> **http://161.153.109.111:5001**



[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4)](https://dotnet.microsoft.com/apps/aspnet)
[![SQLite](https://img.shields.io/badge/SQLite-3-003B57)](https://www.sqlite.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
---

![img.png](DOcs/GoodBurger-Demo.png)


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

![img.png](DOcs/img.png)

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