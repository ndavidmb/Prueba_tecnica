# Plan de Ejecución: Capa de Infraestructura (Infrastructure Layer)

## Contexto del Proyecto
- **Arquitectura:** Clean Architecture / Entity Framework Core.
- **Ubicación de archivos:** `src/Infrastructure/`
- **Objetivo:** Mapear las entidades de dominio a la base de datos relacional aplicando restricciones a nivel de esquema (Fluent API, índices únicos y check constraints).

---

## Reglas de Persistencia a Garantizar
1. **Apuesta Única por Usuario y Partido:** Mapear un índice único compuesto sobre `(UserId, MatchId)` en la tabla de `Bet`.
2. **Restricción de Puntos:** Configurar un Check Constraint en la tabla `Bet` para asegurar que `PointsEarned` solo acepte los valores `0`, `1` o `3`.
3. **Claves Foráneas de Equipos:** Configurar la relación de `Match` con `Team` (Local y Visitante) con restricción de borrado para evitar borrados en cascada no deseados.
4. **Mapeo de Propiedades Privadas:** Configurar EF Core para que lea/escriba propiedades con `private set` (como `PasswordHash` o `PointsEarned`).

---

## Tareas de Desarrollo

### 1. Implementación de Servicios
- Crear la clase `BcryptPasswordHasher.cs` (o con `Microsoft.AspNetCore.Identity`) en `src/Infrastructure/Security/` implementando la interfaz `IPasswordHasher` del Dominio.

### 2. Mapeos con Fluent API (`src/Infrastructure/Persistence/Configurations/`)

#### A. `UserConfiguration.cs`
- Mapear clave primaria `Id`.
- Definir longitud máxima para `Name` y `Email`.
- Índice único en la columna `Email` (`HasIndex(u => u.Email).IsUnique()`).
- Mapear la propiedad `PasswordHash` hacia la columna `password` de la base de datos.

#### B. `TeamConfiguration.cs`
- Clave primaria `Id`.
- Propiedad `Name` como requerida y longitud máxima.

#### C. `MatchConfiguration.cs`
- Configurar relación de FK `LocalTeam` con `DeleteBehavior.Restrict`.
- Configurar relación de FK `VisitorTeam` con `DeleteBehavior.Restrict`.
- Configurar mapeo del Enum `MatchStatus` a string/varchar o entero.
- Check constraint opcional para base de datos: `HasCheckConstraint("CK_Match_DifferentTeams", "[local_team_id] <> [visitor_team_id]")`.

#### D. `BetConfiguration.cs`
- **Índice Único Compuesto:**
  ```csharp
  builder.HasIndex(b => new { b.UserId, b.MatchId }).IsUnique();
  ```
- **Check Constraint de Puntos:**
  ```csharp
  builder.HasCheckConstraint("CK_Bet_PointsEarned", "[points_earned] IN (0, 1, 3)");
  ```
- Configurar claves foráneas hacia `User` y `Match`.

### 3. DbContext y Migraciones
- Registrar las configuraciones en `ApplicationDbContext.cs` mediante `modelBuilder.ApplyConfigurationsFromAssembly(...)`.
- Ejecutar el comando para generar la migración inicial:
  `dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/WebApi`

---

## Checklist de Ejecución para la IA

- [ ] Implementar `IPasswordHasher` en `Infrastructure/Security/PasswordHasher.cs`.
- [ ] Crear `UserConfiguration.cs` asignando índice único al email.
- [ ] Crear `TeamConfiguration.cs`.
- [ ] Crear `MatchConfiguration.cs` con políticas de borrado `Restrict` para equipos local y visitante.
- [ ] Crear `BetConfiguration.cs` con el índice único compuesto `(UserId, MatchId)` y el check constraint de puntos `0, 1, 3`.
- [ ] Registrar las configuraciones en el `DbContext`.
- [ ] Crear y verificar la migración de EF Core (`InitialCreate`).