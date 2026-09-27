# CLAUDE.md — Polla_Futbolera_Backend

Contexto de desarrollo para Claude Code. Leer antes de implementar cualquier funcionalidad.

## Arquitectura

Solución .NET 10 en Clean Architecture. Cuatro proyectos con reglas de dependencia estrictas:

```
Domain ← Application ← Infrastructure ← WebApi
                                    ↑
                         (WebApi también referencia Application)
```

| Proyecto       | Tipo            | Dependencias                  | NuGet                                      |
|----------------|-----------------|-------------------------------|--------------------------------------------|
| Domain         | Class Library   | ninguna                       | ninguno                                    |
| Application    | Class Library   | Domain                        | ninguno                                    |
| Infrastructure | Class Library   | Domain, Application           | EF Core, Npgsql, EF Tools                  |
| WebApi         | Web API         | Application, Infrastructure   | EF Core Design (solo design-time)          |

- **Solo Infrastructure** puede tener Entity Framework u otras dependencias de infraestructura.
- **Domain** no referencia ningún framework externo.
- **Las interfaces de repositorio se definen en Domain** (`Domain/Repositories/IXRepository.cs`). Infrastructure provee las implementaciones concretas con EF Core (`Infrastructure/Repositories/XRepository.cs`).
- Contexto completo en `.ai/context/architecture.md`.

## Convenciones C#

### Primary constructors
Usar siempre que la clase no requiera lógica en el cuerpo del constructor.

```csharp
// Correcto
public class DomainException(string message) : Exception(message);
```

### Enums
Valores en **PascalCase**, no en SCREAMING_SNAKE_CASE.

```csharp
// Correcto
public enum MatchStatus { Pending, InProgress, Finished, Cancelled }
```

Referencia completa en `.ai/context/coding_preferences.md`.

## Comandos frecuentes

```sh
# Build completo
dotnet build Polla_Futbolera_Backend.slnx

# Nueva migración
dotnet ef migrations add <Nombre> \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj

# Aplicar migraciones
dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj
```

## Gestión de planes

- Los planes activos están en `.ai/plans/active/`.
- Al terminar de implementar un plan, moverlo a `.ai/plans/completed/`.

## Contexto adicional

- `.ai/context/architecture.md` — estructura completa de la solución
- `.ai/context/coding_preferences.md` — convenciones y correcciones acordadas
- `.ai/plans/active/` — planes pendientes de implementación
- `.ai/plans/completed/` — planes ya implementados
