# Architecture: Polla_Futbolera_Backend

## Overview

Solución .NET 10 estructurada en Clean Architecture con 4 proyectos. Las capas internas (Domain, Application) no conocen detalles de infraestructura ni de frameworks externos. Solo Infrastructure contiene Entity Framework Core.

## Solution Structure

```
Polla_Futbolera_Backend/
  Polla_Futbolera_Backend.slnx
  src/
    Domain/
      Domain.csproj
      Entities/
        Product.cs
    Application/
      Application.csproj
    Infrastructure/
      Infrastructure.csproj
      Persistence/
        AppDbContext.cs
    WebApi/
      WebApi.csproj
      Program.cs
      appsettings.json
```

## Projects

### Domain
- Tipo: Class Library
- Dependencias: ninguna
- Responsabilidad: entidades del negocio, value objects, interfaces de repositorio

### Application
- Tipo: Class Library
- Dependencias: Domain
- Responsabilidad: casos de uso, comandos, queries, DTOs, interfaces de servicios

### Infrastructure
- Tipo: Class Library
- Dependencias: Domain, Application
- NuGet: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Tools`
- Responsabilidad: implementación de repositorios, DbContext, migraciones, servicios externos

### WebApi
- Tipo: ASP.NET Core Web API (controllers)
- Dependencias: Application, Infrastructure
- NuGet: `Microsoft.EntityFrameworkCore.Design` (solo design-time, para `dotnet ef`)
- Responsabilidad: endpoints HTTP, DI composition root, configuración

## Dependency Rules

```
Domain ← Application ← Infrastructure ← WebApi
                                    ↑
                         (WebApi también referencia Application)
```

WebApi no referencia Domain directamente; lo obtiene de forma transitiva.

## Database

- Proveedor: PostgreSQL via Npgsql
- DbContext: `Infrastructure.Persistence.AppDbContext`
- Connection string en `src/WebApi/appsettings.json` bajo `ConnectionStrings:DefaultConnection`

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=PolleFutboleraDb;Username=postgres;Password=yourpassword"
}
```

## Migrations

```sh
# Crear una migración
dotnet ef migrations add <NombreMigracion> \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj

# Aplicar migraciones a la base de datos
dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj
```

## Build

```sh
dotnet build Polla_Futbolera_Backend.slnx
```
