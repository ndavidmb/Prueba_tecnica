# Plan de Ejecución: Módulo 1 - Autenticación y Usuarios

## Contexto del Proyecto
- **Arquitectura:** Clean Architecture / Layered Architecture.
- **Seguridad:** Autenticación basada en JWT con esquema Bearer.
- **Roles:** `User` (realiza pronósticos) y `Admin` (gestiona partidos/resultados).
- **Patrón:** Repository Pattern (Interfaz en `Domain`, Implementación en `Infrastructure`).

---

## 1. Definiciones por Capa

### A. Capa `Domain` (`src/Domain/`)
- **Abstracción del Repositorio (`IUserRepository.cs`):** Define el contrato para persistir y consultar usuarios.
  - `Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);`
  - `Task AddAsync(User user, CancellationToken cancellationToken = default);`
  - `Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);`
- **Abstracción de Token JWT (`IJwtTokenGenerator.cs`):**
  - `string GenerateToken(User user);`

### B. Capa `Application` (`src/Application/`)
- **DTOs (`src/Application/DTOs/Auth/`):**
  - `RegisterUserDto` (`Name`, `Email`, `Password`, `Role` [opcional/por defecto 'User']).
  - `LoginDto` (`Email`, `Password`).
  - `AuthResponseDto` (`Token`, `Email`, `Role`, `Name`).
- **Casos de Uso / Servicios (`src/Application/Services/`):**
  - **`IAuthService`:**
    - `Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);`
    - `Task<AuthResponseDto> LoginAsync(LoginDto dto);`
  - **`AuthService` (Implementación):**
    - `RegisterAsync`: Verifica que el email no exista en `IUserRepository`, llama al método/fábrica de dominio para hashear la contraseña (`IPasswordHasher`), guarda el usuario usando `IUserRepository` y genera el token con `IJwtTokenGenerator`.
    - `LoginAsync`: Busca al usuario con `IUserRepository`, valida la contraseña usando `IPasswordHasher`, lanza excepción si es inválida y devuelve el `AuthResponseDto` con el token JWT generado.

### C. Capa `Infrastructure` (`src/Infrastructure/`)
- **Implementación del Repositorio (`UserRepository.cs`):**
  - Implementa `IUserRepository` usando `ApplicationDbContext` y Entity Framework Core.
- **Servicio de JWT (`JwtTokenGenerator.cs`):**
  - Implementa `IJwtTokenGenerator` generando los claims requeridos (`ClaimTypes.NameIdentifier`, `ClaimTypes.Email`, `ClaimTypes.Role`).
  - Configura la firma digital mediante la clave secreta leída desde la configuración (`appsettings.json`).

### D. Capa `WebApi` (`src/WebApi/`)
- **Configuración de Servicios (`Program.cs` / Dependency Injection):**
  - Registrar autenticación JWT (`AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`).
  - Configurar políticas de autorización si aplica.
  - Inyectar dependencias: `IUserRepository` -> `UserRepository`, `IJwtTokenGenerator` -> `JwtTokenGenerator`, `IAuthService` -> `AuthService`, `IPasswordHasher` -> `PasswordHasher`.
- **Controlador (`Controllers/AuthController.cs`):**
  - `POST api/auth/register`: Inyecta `IAuthService` y llama a `RegisterAsync`. Retorna `201 Created` o `200 OK`.
  - `POST api/auth/login`: Inyecta `IAuthService` y llama a `LoginAsync`. Retorna `200 OK` con la respuesta del token.

---

## 2. Checklist de Ejecución para la IA

- [ ] **Paso 1 (Domain):** Crear la interfaz `IUserRepository.cs` en `src/Domain/Interfaces/`.
- [ ] **Paso 2 (Domain):** Crear la interfaz `IJwtTokenGenerator.cs` en `src/Domain/Interfaces/`.
- [ ] **Paso 3 (Application):** Crear los DTOs `RegisterUserDto.cs`, `LoginDto.cs` y `AuthResponseDto.cs` en `src/Application/DTOs/Auth/`.
- [ ] **Paso 4 (Application):** Crear la interfaz `IAuthService.cs` y su implementación `AuthService.cs` en `src/Application/Services/`.
- [ ] **Paso 5 (Infrastructure):** Crear `UserRepository.cs` implementando `IUserRepository` con EF Core en `src/Infrastructure/Persistence/Repositories/`.
- [ ] **Paso 6 (Infrastructure):** Crear `JwtTokenGenerator.cs` implementando `IJwtTokenGenerator` con `System.IdentityModel.Tokens.Jwt` en `src/Infrastructure/Security/`.
- [ ] **Paso 7 (WebApi):** Configurar autenticación Bearer JWT en `Program.cs` y registrar los servicios en el contenedor de DI.
- [ ] **Paso 8 (WebApi):** Crear `AuthController.cs` consumiendo `IAuthService` con los endpoints `POST /register` y `POST /login`.