# Plan de Ejecución: Módulo 3 - Panel de Administración y Control de Partidos

## Contexto del Proyecto
- **Arquitectura:** Clean Architecture / Layered Architecture.
- **Seguridad:** Control de Acceso Basado en Roles (`[Authorize(Roles = "Admin")]`).
- **Estados de Partido:** Enumeración `MatchStatus` (`UpcomingMatch`, `FullTime`).
- **Invariante de Negocio Principal:** Un usuario **solo puede realizar apuestas** en partidos con estado `UpcomingMatch`.

---

## 1. Ajustes en el Modelo de Dominio (`Domain`)

### A. Enum `MatchStatus.cs` (`src/Domain/Enums/`)
```csharp
public enum MatchStatus
{
    UpcomingMatch = 1,
    FullTime = 2
}
```

### B. Entidad `Match.cs` (`src/Domain/Entities/`)
- Restablecer la propiedad `Status` de tipo `MatchStatus` con un estado inicial predeterminado `MatchStatus.UpcomingMatch`.
- Agregar o actualizar los campos de marcador: `LocalGoals` y `VisitorGoals` (pueden ser nulos mientras esté en `UpcomingMatch`).
- Método de Dominio `UpdateResult(int localGoals, int visitorGoals)`:
  - Valida que `localGoals >= 0` y `visitorGoals >= 0`.
  - Establece los goles `LocalGoals` y `VisitorGoals`.
  - Modifica el estado a `MatchStatus.FullTime`.

### C. Regla de Negocio en `BetService` / `Bet.cs`
- En el caso de uso de registrar apuesta (`PlaceBetAsync`), se consulta el partido y se agrega la validación:
  ```csharp
  if (match.Status != MatchStatus.UpcomingMatch)
  {
      throw new DomainException("No se pueden realizar apuestas en un partido que no esté pendiente (UpcomingMatch).");
  }
  ```

---

## 2. Definiciones por Capa

### A. Capa `Application` (`src/Application/`)

* **DTOs (`src/Application/DTOs/Matches/`):**
  - `UpdateMatchResultDto` (`int LocalGoals`, `int VisitorGoals`).
  - `MatchResponseDto` (`int Id`, `int LocalTeamId`, `int VisitorTeamId`, `int? LocalGoals`, `int? VisitorGoals`, `MatchStatus Status`).

* **Casos de Uso / Servicio (`IMatchService.cs` y `MatchService.cs`):**
  - Método `Task<MatchResponseDto> UpdateResultAsync(int matchId, UpdateMatchResultDto dto)`:
    1. Obtiene el partido de `IMatchRepository.GetByIdAsync(matchId)`.
    2. Ejecuta el método de dominio `match.UpdateResult(dto.LocalGoals, dto.VisitorGoals)`.
    3. Persiste los cambios llamando a `IMatchRepository.UpdateAsync(match)`.
    4. (Opcional/Futuro) Recalcula o actualiza puntos de las apuestas ligadas a este partido si fuera necesario diferir la puntuación.
    5. Retorna `MatchResponseDto`.

### B. Capa `Infrastructure` (`src/Infrastructure/`)

* **Mapeo Fluent API (`MatchConfiguration.cs`):**
  - Mapear la propiedad `Status` para almacenarse como string o entero en la base de datos.
* **Repositorio (`MatchRepository.cs`):**
  - Garantizar el método `UpdateAsync(Match match)` para guardar los cambios del partido en EF Core.

### C. Capa `WebApi` (`src/WebApi/`)

* **Controlador (`Controllers/AdminMatchesController.cs` o `MatchesController.cs`):**
  - `PUT api/admin/matches/{id}/result`
    - Atributo de seguridad: `[Authorize(Roles = "Admin")]`
    - Recibe el `id` del partido y el body con `UpdateMatchResultDto`.
    - Ejecuta `IMatchService.UpdateResultAsync`.
    - Retorna `200 OK` con la información del partido actualizado.

---

## 3. Checklist de Ejecución para la IA

- [ ] **Paso 1 (Domain):** Crear el enum `MatchStatus.cs` con los valores `UpcomingMatch` y `FullTime`.
- [ ] **Paso 2 (Domain):** Actualizar `Match.cs` agregando la propiedad `Status` de tipo `MatchStatus` y el método `UpdateResult(localGoals, visitorGoals)`.
- [ ] **Paso 3 (Application):** Modificar `BetService.cs` para rechazar apuestas en partidos que no tengan el estado `MatchStatus.UpcomingMatch`.
- [ ] **Paso 4 (Application):** Crear `UpdateMatchResultDto.cs` y `MatchResponseDto.cs`.
- [ ] **Paso 5 (Application):** Crear la interfaz `IMatchService.cs` y su implementación `MatchService.cs` con el método `UpdateResultAsync`.
- [ ] **Paso 6 (Infrastructure):** Actualizar `MatchConfiguration.cs` para el mapeo del nuevo enum `MatchStatus`.
- [ ] **Paso 7 (WebApi):** Crear o actualizar el controlador de partidos con el endpoint `PUT api/admin/matches/{id}/result` protegido exclusivamente para el rol `Admin`.