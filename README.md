# BiografWebsiteApiDb

## Backend (ASP.NET Core 9 + SQL Server)
- Prerequisites: .NET 9 SDK and access to a SQL Server instance.
- Configure the connection string with `ConnectionStrings__DefaultConnection` (recommended) or edit `Backend/BiografOpgave.API/appsettings.json`. Example (PowerShell):
  ```powershell
  $env:ConnectionStrings__DefaultConnection="Server=localhost;Database=BiografOpgave;User ID=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=True"
  ```
- First-time setup: apply migrations to create/update the database (requires the `dotnet-ef` tool):
  ```bash
  dotnet ef database update --project Backend/BiografOpgave.Infrastructure --startup-project Backend/BiografOpgave.API
  ```
- Run the API:
  ```bash
  dotnet run --project Backend/BiografOpgave.API
  ```
  Launch settings expose the app on `http://localhost:5104` (and `https://localhost:7004`), which matches the Angular services’ `baseUrl` values.

## Frontend (Angular 20)
- Prerequisites: Node.js (LTS recommended) and npm.
- Install dependencies:
  ```bash
  cd Frontend
  npm install
  ```
- Run the dev server (defaults to `http://localhost:4200`):
  ```bash
  npm start   # or: ng serve
  ```
