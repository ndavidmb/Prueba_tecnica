# Plan de Ejecución: Módulo 4 - Leaderboard y Dashboard

## Contexto del Proyecto
- **Arquitectura:** Clean Architecture / Layered Architecture.
- **Enfoque:** Consultas optimizadas de lectura para soporte de interfaz visual (Dashboard / Leaderboard).
- **Seguridad:** Endpoints accesibles para usuarios autenticados (`[Authorize]`).

---

## 1. Casos de Uso Planificados

### Caso de Uso 1: Consulta de Tabla de Posiciones Global (Ranking)
Obtiene la lista de usuarios con sus puntos totales sumados a partir de sus apuestas, ordenada en forma descendente (de mayor a menor puntuación).

### Caso de Uso 2: Consulta Histórica de Apuestas por Usuario
Dado un `userId`, obtiene el detalle de todas sus apuestas realizadas, incluyendo los goles pronosticados, los goles reales del partido, los equipos enfrentados y los puntos obtenidos por cada apuesta.

---

## 2. Definiciones por Capa

### A. Capa `Domain` (`src/Domain/`)

* **Extensión de Repositorios:**
  * **`IUserRepository.cs` / `IBetRepository.cs`:**
    * `Task<IEnumerable<UserLeaderboardDto>> GetLeaderboardAsync(CancellationToken cancellationToken = default);`
    * `Task<UserHistoryDto?> GetUserBetHistoryAsync(int userId, CancellationToken cancellationToken = default);`

### B. Capa `Application` (`src/Application/`)

* **DTOs (`src/Application/DTOs/Leaderboard/`):**

  * **`UserLeaderboardDto.cs`:**
    * `int UserId`
    * `string UserName`
    * `int TotalPoints`
    * `int TotalBets`
    * `int RankPosition`

  * **`UserBetHistoryItemDto.cs`:**
    * `int BetId`
    * `int MatchId`
    * `string LocalTeamName`
    * `string VisitorTeamName`
    * `int PredictedLocalGoals`
    * `int PredictedVisitorGoals`
    * `int? RealLocalGoals`
    * `int? RealVisitorGoals`
    * `int PointsEarned`

  * **`UserHistoryDto.cs`:**
    * `int UserId`
    * `string UserName`
    * `int TotalPoints`
    * `List<UserBetHistoryItemDto> Bets`

* **Servicio (`ILeaderboardService.cs` y `LeaderboardService.cs`):**
  * `Task<IEnumerable<UserLeaderboardDto>> GetLeaderboardAsync();`
  * `Task<UserHistoryDto>` `GetUserHistoryAsync(int userId);`

---

### C. Capa `Infrastructure` (`src/Infrastructure/`)

* **Optimización de Consultas (EF Core / LINQ):**

  * **Ranking Global:**
    Consulta agregada sobre la tabla de usuarios y apuestas:
    ```csharp
    context.Users
        .Select(u => new UserLeaderboardDto {
            UserId = u.Id,
            UserName = u.Name,
            TotalPoints = u.Bets.Sum(b => b.PointsEarned),
            TotalBets = u.Bets.Count()
        })
        .OrderByDescending(u => u.TotalPoints)
        .ToListAsync();
    ```

  * **Historial por Usuario:**
    Consulta con `.Include()` o proyección directa a DTO conectando `Bet`, `Match` y `Team` (Local y Visitante) filtrando por `UserId`.

---

### D. Capa `WebApi` (`src/WebApi/`)

* **Controlador (`Controllers/LeaderboardController.cs`):**

  1. **`GET api/leaderboard`**
     * Atributo: `[Authorize]`
     * Invoca `ILeaderboardService.GetLeaderboardAsync()`
     * Retorna `200 OK` con la lista de usuarios y sus puntos ordenados.

  2. **`GET api/leaderboard/users/{userId}/history`**
     * Atributo: `[Authorize]`
     * Invoca `ILeaderboardService.GetUserHistoryAsync(userId)`
     * Retorna `200 OK` con el historial del usuario o `404 Not Found` si el usuario no existe.

---

## 3. Checklist de Ejecución para la IA

- [ ] **Paso 1 (Application):** Crear los DTOs `UserLeaderboardDto.cs`, `UserBetHistoryItemDto.cs` y `UserHistoryDto.cs` en `src/Application/DTOs/Leaderboard/`.
- [ ] **Paso 2 (Domain/Infrastructure):** Definir los contratos de consulta en las interfaces e implementar las consultas agregadas optimizadas en EF Core dentro de `Infrastructure`.
- [ ] **Paso 3 (Application):** Crear la interfaz `ILeaderboardService.cs` y su implementación `LeaderboardService.cs`.
- [ ] **Paso 4 (WebApi):** Crear `LeaderboardController.cs` expuesto con los endpoints `GET /api/leaderboard` y `GET /api/leaderboard/users/{userId}/history`.
- [ ] **Paso 5 (WebApi):** Registrar `ILeaderboardService` en la inyección de dependencias (`Program.cs`).