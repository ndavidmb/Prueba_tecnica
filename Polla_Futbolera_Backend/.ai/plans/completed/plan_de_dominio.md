# Plan de Ejecución: Capa de Dominio (Domain Layer)

## Contexto del Proyecto
- **Arquitectura:** Clean Architecture / Domain-Driven Design (DDD).
- **Ubicación de archivos:** `src/Domain/`
- **Objetivo:** Encapsular todas las entidades, invariantes y reglas de negocio sin dependencias externas (sin framework ORM ni librerías de infraestructura).

---

## Reglas e Invariantes de Negocio a Implementar
1. **Validaciones de Negocio:** Las entidades deben validar sus propios datos al instanciarse o modificarse, lanzando excepciones de dominio en caso de estado inválido.
2. **Encapsulamiento de Contraseña (`User`):** La propiedad `PasswordHash` posee un `private set`. Su actualización solo se realiza mediante un método explícito de dominio o en el constructor pasando un contrato/servicio de hashing (`IPasswordHasher`).
3. **Rivalidad Única (`Match`):** Un partido no puede tener el mismo equipo como local y visitante (`LocalTeamId != VisitorTeamId`).
4. **Validación de Puntos (`Bet`):** Los puntos asignados a una apuesta solo pueden ser exactamente `0`, `1` o `3`.
5. **Goles Válidos:** Los goles pronosticados o reales no pueden ser valores negativos.

---

## Tareas de Desarrollo

### 1. Definición de Enums e Interfaces
- Crear `Domain/Enums/MatchStatus.cs` (`PENDING`, `IN_PROGRESS`, `FINISHED`, `CANCELLED`).
- Crear `Domain/Services/IPasswordHasher.cs` con la abstracción para encriptar/verificar contraseñas (`HashPassword(string plainText)`, `VerifyPassword(string plainText, string hash)`).

### 2. Implementación de Entidades (`src/Domain/Entities/`)

#### A. `User.cs`
- Propiedades: `Id`, `Name`, `Email`, `PasswordHash`, `Role`.
- Setters privados para asegurar inmutabilidad no controlada.
- Método de fábrica `Create(string name, string email, string plainPassword, IPasswordHasher hasher)` para asegurar que la contraseña siempre pase por el proceso de hashing antes de asignarse.
- Método de actualización `UpdatePassword(string newPlainPassword, IPasswordHasher hasher)`.

#### B. `Team.cs`
- Propiedades: `Id`, `Name`.
- Constructor/Método de creación que valida que `Name` no sea nulo ni vacío.

#### C. `Match.cs`
- Propiedades: `Id`, `LocalTeamId`, `VisitorTeamId`, `MatchDate`, `LocalGoals` (nullable), `VisitorGoals` (nullable), `Status` (`MatchStatus`).
- Validación en el constructor: `if (localTeamId == visitorTeamId) throw new DomainException("Un equipo no puede jugar contra sí mismo.");`
- Método `SetResult(int localGoals, int visitorGoals)` que valida goles $\ge 0$ y cambia el estado a `MatchStatus.FINISHED`.

#### D. `Bet.cs`
- Propiedades: `Id`, `UserId`, `MatchId`, `LocalGoals`, `VisitorGoals`, `PointsEarned`.
- Validación en constructor: `LocalGoals >= 0` y `VisitorGoals >= 0`.
- Método `AssignPoints(int points)`:
  - Válida que `points` sea únicamente `0`, `1` o `3`. De lo contrario, lanza excepción de dominio.

---

## Checklist de Ejecución para la IA

- [ ] Crear el Enum `MatchStatus.cs` en `src/Domain/Enums/`.
- [ ] Crear la interfaz `IPasswordHasher.cs` en `src/Domain/Services/` o `src/Domain/Interfaces/`.
- [ ] Crear la clase base/excepción de dominio `DomainException.cs`.
- [ ] Implementar la entidad `Team.cs` con validación de nombre.
- [ ] Implementar la entidad `User.cs` con hashing encapsulado en constructor/fábrica.
- [ ] Implementar la entidad `Match.cs` validando `LocalTeamId != VisitorTeamId`.
- [ ] Implementar la entidad `Bet.cs` validando goles $\ge 0$ y restricción de puntos (`0`, `1`, `3`).