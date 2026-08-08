# HelpDeskManagement

Help Desk Ticket Management System built with **ASP.NET Core Web API**, **ASP.NET Core MVC**, **Entity Framework Core**, **SQL Server**, **ASP.NET Core Identity**, **xUnit**, and **Moq**.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-blueviolet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver&logoColor=white)
![xUnit](https://img.shields.io/badge/tests-xUnit%20%2B%20Moq-25A162)
![License](https://img.shields.io/badge/license-MIT-green)

A company receives support requests from employees regarding software, hardware, and network issues. This system lets employees raise, track, and close their own tickets, while a Help Desk admin has full visibility and control over every ticket.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [1. Clone & Restore](#1-clone--restore)
  - [2. Apply Database Migrations](#2-apply-database-migrations)
  - [3. Trust the Local Dev Certificate](#3-trust-the-local-dev-certificate-optional)
  - [4. Run the API](#4-run-the-api)
  - [5. Run the MVC App](#5-run-the-mvc-app)
  - [6. Run the Tests](#6-run-the-tests)
- [Roles & Access Control](#roles--access-control)
- [API Reference](#api-reference)
- [Ticket Field Reference](#ticket-field-reference)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

---

## Features

- **Raise, view, edit, close, and delete tickets** as a regular user — scoped to only the tickets you raised.
- **Admin Panel** for Help Desk staff — see every ticket in the system and move it between `Open`, `In Progress`, and `Closed`, or delete it outright.
- **Login & Registration** via ASP.NET Core Identity, with role-based access (`Admin` / `User`).
- **Dashboard** showing live totals (Total / Open / In Progress / Closed), scoped per role.
- **Filter by status** with a dropdown, radio buttons, or navigation links.
- Clean separation of concerns: **Repository Pattern** in the API, **Service Layer** in the MVC app — MVC never touches the ticket database directly.
- **Unit tested** controller logic using xUnit + Moq, with the repository layer fully mocked (no SQL Server dependency in tests).

## Architecture

```
┌─────────────────┐        HTTP (JSON)        ┌──────────────────┐        EF Core        ┌────────────────┐
│   HelpDesk.Mvc   │ ────────────────────────▶ │   HelpDesk.Api   │ ─────────────────────▶ │  SQL Server     │
│  (Razor Views,   │ ◀──────────────────────── │ (REST endpoints, │ ◀───────────────────── │  HelpDeskDb     │
│  TicketService,  │      Ticket data only      │  Repository      │      Ticket table       │  (Tickets)      │
│  Identity/Auth)  │                            │  Pattern)         │                        └────────────────┘
└─────────────────┘                            └──────────────────┘
        │
        │ EF Core (separate context)
        ▼
┌────────────────────┐
│  HelpDeskIdentityDb │   ← Login/roles only (AspNetUsers, AspNetRoles, ...)
└────────────────────┘
```

`HelpDesk.Mvc` controllers never query the ticket database directly — all ticket reads/writes go through `TicketService` (an `HttpClient` wrapper) to `HelpDesk.Api`. Login and role data live in a completely separate database so authentication concerns stay decoupled from ticket data.

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 8 Web API, Entity Framework Core 8, SQL Server |
| MVC | ASP.NET Core 8 MVC, Razor Views, Bootstrap 5, ASP.NET Core Identity |
| Testing | xUnit, Moq |
| Source Control | Git / GitHub |

## Project Structure

```
HelpDeskManagement/
├── HelpDesk.Api/                  # ASP.NET Core Web API
│   ├── Controllers/TicketController.cs
│   ├── Data/AppDbContext.cs
│   ├── Models/Ticket.cs
│   └── Repositories/              # ITicketRepository / TicketRepository
├── HelpDesk.Mvc/                  # ASP.NET Core MVC (admin + user-facing UI)
│   ├── Controllers/               # Home, Ticket, Admin, Account
│   ├── Data/                      # Identity DbContext + role/admin seeder
│   ├── Models/                    # Ticket, Dashboard, Login/Register view models
│   ├── Services/                  # ITicketService / TicketService (HttpClient)
│   └── Views/
├── HelpDesk.Tests/                # xUnit + Moq unit tests
│   └── TicketControllerTests.cs
├── HelpDeskManagement.sln
├── .gitignore
├── LICENSE
└── README.md
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

### 1. Clone & Restore

```bash
git clone https://github.com/<your-username>/HelpDeskManagement.git
cd HelpDeskManagement
dotnet restore
dotnet build
```

### 2. Apply Database Migrations

Two separate databases are used — one for tickets (owned by the API), one for logins/roles (owned by the MVC app).

**Ticket database** (from `HelpDesk.Api`):

```bash
cd HelpDesk.Api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Identity database** (from `HelpDesk.Mvc`):

```bash
cd ../HelpDesk.Mvc
dotnet ef migrations add InitialIdentity
dotnet ef database update
```

### 3. Trust the Local Dev Certificate (optional)

Only needed if you switch the projects back to HTTPS; the default `launchSettings.json` in this repo runs both apps over plain HTTP on fixed ports to keep local setup friction-free.

```bash
dotnet dev-certs https --trust
```

### 4. Run the API

```bash
cd HelpDesk.Api
dotnet run
```

Listens on `http://localhost:5001`. Swagger UI is available at `http://localhost:5001/swagger`.

### 5. Run the MVC App

In a **second terminal** (the API must stay running):

```bash
cd HelpDesk.Mvc
dotnet run
```

Listens on `http://localhost:5002`. Open it in your browser — you'll land on the login page.

**Default seeded admin account:**

| Email | Password |
|---|---|
| `admin@helpdesk.com` | `Admin@123` |

> Change or remove this seeded account before deploying anywhere beyond local development.

### 6. Run the Tests

```bash
cd HelpDesk.Tests
dotnet test
```

All controller tests run against a mocked `ITicketRepository` — no database connection required.

## Roles & Access Control

| Capability | User | Admin |
|---|:---:|:---:|
| Raise a new ticket | ✅ | ✅ |
| View own tickets | ✅ | ✅ |
| View **all** tickets | ❌ | ✅ |
| Edit own ticket (title/description/priority) | ✅ | ✅ |
| Close own ticket | ✅ | ✅ |
| Set any ticket to **In Progress** | ❌ | ✅ |
| Reopen a ticket | ❌ | ✅ |
| Delete own ticket | ✅ | ✅ |
| Delete **any** ticket | ❌ | ✅ |

New accounts created via `/Account/Register` are assigned the `User` role automatically. The `Admin` role is seeded once on first application startup and can only be granted manually (e.g. via the database) afterward.

## API Reference

Base URL: `http://localhost:5001`

| HTTP Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/Ticket/All` | Get all tickets |
| `GET` | `/api/Ticket/{id}` | Get a ticket by Id |
| `POST` | `/api/Ticket` | Create a new ticket |
| `PUT` | `/api/Ticket/{id}` | Update an existing ticket |
| `DELETE` | `/api/Ticket/{id}` | Delete a ticket |
| `GET` | `/api/Ticket/Status/{status}` | Get all tickets with a given status |

Full interactive documentation is available via Swagger at `/swagger` while the API is running.

## Ticket Field Reference

| Field | Type | Notes |
|---|---|---|
| `Id` | `int` | Primary key, generated by the database |
| `Title` | `string` | Required |
| `Description` | `string` | Optional |
| `Priority` | `string` | One of: `Low`, `Medium`, `High` |
| `Status` | `string` | One of: `Open`, `In Progress`, `Closed` |
| `RaisedBy` | `string` | Set automatically to the logged-in user; not client-editable |
| `CreatedDate` | `DateTime` | Set automatically on creation |

## Configuration

Connection strings and the API base URL are set per project:

**`HelpDesk.Api/appsettings.json`**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HelpDeskDb;Trusted_Connection=True;..."
  }
}
```

**`HelpDesk.Mvc/appsettings.json`**
```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=(localdb)\\mssqllocaldb;Database=HelpDeskIdentityDb;Trusted_Connection=True;..."
  },
  "ApiSettings": {
    "BaseUrl": "http://localhost:5001/"
  }
}
```

For anything beyond local development, move connection strings and secrets out of `appsettings.json` and into [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables.

## Contributing

1. Fork the repository and create a feature branch: `git checkout -b feature/my-feature`
2. Commit your changes with clear, descriptive messages
3. Push to your branch and open a Pull Request

Please keep PRs focused on a single change and include a short description of what changed and why.

## License

Distributed under the [MIT License](LICENSE).
