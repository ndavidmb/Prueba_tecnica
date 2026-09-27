# Plan de Ejecución: Módulo de Apuestas / Pronósticos (Betting Module)

## Contexto del Proyecto
- **Arquitectura:** Clean Architecture / Layered Architecture.
- **Flujo:** Creación de apuesta + Cálculo instantáneo de puntos respecto al resultado del partido.
- **Ajustes de Modelo (`Match`):** Se eliminan `MatchDate` y `Status` como campos requeridos (o se remueven de la entidad).

---

## 1. Ajuste al Modelo de Dominio (`Match`)
- **Actualización de `Match.cs`:**
  - Remover obligatoriedad de `MatchDate` y `Status`.
  - Simplificar las propiedades de `Match`:
    - `Id` (int / Guid)
    - `LocalTeamId` (int)
    - `VisitorTeamId` (int)
    - `LocalGoals` (int) — Marcador real del local.
    - `VisitorGoals` (int) — Marcador real del visitante.

---

## 2. Lógica de Negocio de Apuestas y Puntuación

### Algoritmo de Cálculo de Puntos (`Bet` / Domain Service)
Dado un pronóstico (`Bet`) y un resultado real (`Match`):
- **3 Puntos (Acierto Exacto):** Los goles del local y visitante pronosticados coinciden exactamente con los goles reales.
  - Ej: Pronóstico `2 - 1`, Resultado Real `2 - 1` $\rightarrow$ **3 puntos**.
- **1 Punto (Acierto de Tendencia):** Se acierta al ganador o al empate, pero no al resultado exacto.
  - Ej: Pronóstico `3 - 1` (Gana local), Resultado Real `1 - 0` (Gana local) $\rightarrow$ **1 punto**.
  - Ej: Pronóstico `2 - 2` (Empate), Resultado Real `0 - 0` (Empate) $\rightarrow$ **1 punto**.
- **0 Puntos (Fallo):** No se acertó ni la tendencia ni el marcador exacto.

---

## 3. Definiciones por Capa

### A. Capa `Domain` (`src/Domain/`)

* **Ajuste en `Match.cs`:**
  - Actualizar el constructor/métodos para no exigir `MatchDate` ni `Status`.
* **Regla en `Bet.cs`:**
  - Método `CalculateAndAssignPoints(int realLocalGoals, int realVisitorGoals)` que ejecuta la lógica de scoring ($0, 1, 3$) y actualiza `PointsEarned`.
* **Abstracciones de Repositorios:**
  - `IBetRepository.cs`:
    - `Task<bool> ExistsByUserAndMatchAsync(int userId, int matchId, CancellationToken cancellationToken = default);`
    - `Task AddAsync(Bet bet, CancellationToken cancellationToken = default);`
  - `IMatchRepository.cs`:
    - `Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default);`

### B. Capa `Application` (`src/Application/`)

* **DTOs (`src/Application/DTOs/Bets/`):**
  - `CreateBetDto` (`MatchId`, `LocalGoals`, `VisitorGoals`).
  - `BetResultDto`:
    - `BetId`
    - `UserId`
    - `MatchId`
    - `PredictedLocalGoals`, `PredictedVisitorGoals`
    - `RealLocalGoals`, `RealVisitorGoals`
    - `PointsEarned` (0, 1, 3)
    - `IsExactMatch` (bool - Acierto exacto)
    - `IsTrendMatch` (bool - Acierto de ganador/empate)

* **Caso de Uso / Servicio (`IBetService.cs` y `BetService.cs`):**
  - Método `Task<BetResultDto> PlaceBetAsync(int userId, CreateBetDto dto)`:
    1. Validar existencia del partido (`IMatchRepository.GetByIdAsync`).
    2. Validar que el usuario **no tenga una apuesta existente** para ese partido (`IBetRepository.ExistsByUserAndMatchAsync`). Si existe, lanzar `DomainException` o `InvalidOperationException`.
    3. Crear la entidad `Bet`.
    4. Calcular inmediatamente los puntos llamando a `bet.CalculateAndAssignPoints(match.LocalGoals, match.VisitorGoals)`.
    5. Guardar la apuesta en `IBetRepository`.
    6. Retornar `BetResultDto` indicando el desglose del resultado obtenido.

### C. Capa `Infrastructure` (`src/Infrastructure/`)

* **Repositorios:**
  - Implementar `BetRepository.cs` con EF Core.
  - Implementar `MatchRepository.cs` con EF Core.
* **Configuración Fluent API (`MatchConfiguration.cs`):**
  - Quitar las restricciones obligatorias para `MatchDate` y `Status`.

### D. Capa `WebApi` (`src/WebApi/`)

* **Controlador (`Controllers/BetsController.cs`):**
  - `POST api/bets` (Protegido con `[Authorize]` para obtener el `UserId` del Token JWT).
  - Recibe `CreateBetDto`, ejecuta `IBetService.PlaceBetAsync` y retorna `201 Created` / `200 OK` con la evaluación inmediata de los puntos.

---

## 4. Checklist de Ejecución para la IA

- [ ] **Paso 1 (Domain):** Modificar `Match.cs` eliminando la obligatoriedad de `MatchDate` y `Status`.
- [ ] **Paso 2 (Domain):** Implementar la lógica de cálculo de puntos dentro de `Bet.cs` o mediante un servicio de dominio (`0`, `1` o `3` puntos).
- [ ] **Paso 3 (Domain):** Crear las interfaces `IBetRepository.cs` y `IMatchRepository.cs`.
- [ ] **Paso 4 (Application):** Crear los DTOs `CreateBetDto.cs` y `BetResultDto.cs`.
- [ ] **Paso 5 (Application):** Crear la interfaz `IBetService.cs` y su implementación `BetService.cs` procesando la validación de duplicados y cálculo instantáneo.
- [ ] **Paso 6 (Infrastructure):** Actualizar `MatchConfiguration.cs` e implementar `BetRepository.cs` y `MatchRepository.cs`.
- [ ] **Paso 7 (WebApi):** Crear `BetsController.cs` con el endpoint `POST api/bets` inyectando el `UserId` desde el token JWT.