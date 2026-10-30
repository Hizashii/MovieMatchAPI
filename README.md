# MovieMatch API

A C# / **ASP.NET Core (.NET 8)** REST API project for movie data and user authentication. Built to explore API structure, dependency injection, service and repository abstractions, and Swagger documentation.

## Stack

- ASP.NET Core Web API and C#
- JWT bearer authentication
- Swagger / OpenAPI
- Entity Framework Core and SQLite configuration
- In-memory repository implementations

## Architecture

- `Controllers/` — HTTP endpoints
- `Services/` — application services
- `Repositories/` — repository abstractions and implementations
- `Models/` — application data models
- `Data/` — database context
- `Filters/` — request filters

## Running locally

Install the **.NET 8 SDK** and restore dependencies:

```bash
dotnet restore
dotnet run
```

The root route redirects to **/swagger** for API exploration.

The application reads configuration for its JWT issuer, audience and signing key. Supply a strong local secret using secure configuration; don't use development defaults in a deployment.

## Persistence: current implementation

`Program.cs` includes a `UseInMemory` setting and an Entity Framework Core / SQLite database context. **Movie and user repositories currently remain in-memory in both modes.** The database configuration is a foundation for future persistent repository implementations, not a claim that movie records are already persisted to SQLite.

## What this project shows

- REST endpoint design
- Middleware and JWT authentication configuration
- Dependency injection and interfaces
- API exploration through Swagger
- Trade-offs between an in-memory prototype and durable persistence

This is a learning/portfolio API, not a production service.
