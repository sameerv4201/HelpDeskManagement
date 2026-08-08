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
