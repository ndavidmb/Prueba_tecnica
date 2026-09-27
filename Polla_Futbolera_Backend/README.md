# Polla Futbolera Backend

API REST construida con .NET 10 y Clean Architecture. Permite gestionar usuarios, equipos, partidos y apuestas de resultados.

## Ejecución con Docker (recomendado)

Requiere [Docker](https://www.docker.com/get-started) con Docker Compose.

```bash
# Construir y levantar la API + PostgreSQL
docker compose up --build
```

La API queda disponible en `http://localhost:8080`.

### Migraciones en Docker

```bash
# Crear migración inicial
docker compose exec api dotnet ef migrations add InitialCreate \
  --project src/Infrastructure \
  --startup-project src/WebApi

# Aplicar migraciones
docker compose exec api dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/WebApi
```

### Detener los contenedores

```bash
docker compose down        # detiene y elimina contenedores
docker compose down -v     # también elimina el volumen de PostgreSQL
```

---

## Ejecución local

### Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (local o en contenedor)
- [dotnet-ef CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (para migraciones)

```bash
dotnet tool install --global dotnet-ef
```

## Configuración

### 1. Base de datos

Crea una base de datos PostgreSQL y actualiza la cadena de conexión en `src/WebApi/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=PolleFutboleraDb;Username=postgres;Password=yourpassword"
}
```

### 2. JWT

Reemplaza `SecretKey` por una cadena aleatoria de al menos 32 caracteres:

```json
"Jwt": {
  "SecretKey": "TU_CLAVE_SECRETA_LARGA_Y_ALEATORIA",
  "Issuer": "PollaFutboleraApi",
  "Audience": "PollaFutboleraClient"
}
```

## Migraciones

Desde la carpeta `Polla_Futbolera_Backend/`:

```bash
# Crear la migración inicial
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj

# Aplicar la migración a la base de datos
dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/WebApi/WebApi.csproj
```

## Ejecución

Desde la carpeta `Polla_Futbolera_Backend/`:

```bash
dotnet run --project src/WebApi/WebApi.csproj
```

La API queda disponible en:

| Perfil | URL |
|--------|-----|
| HTTP   | http://localhost:5212 |
| HTTPS  | https://localhost:7296 |

Para usar HTTPS:

```bash
dotnet run --project src/WebApi/WebApi.csproj --launch-profile https
```

## Documentación interactiva (Scalar)

En modo desarrollo la UI interactiva está disponible en:

```
http://localhost:5212/scalar/v1
```

Desde ahí puedes explorar y ejecutar todos los endpoints directamente en el navegador. La especificación OpenAPI en JSON está en:

```
http://localhost:5212/openapi/v1.json
```

## Endpoints disponibles

### Autenticación

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/auth/register` | Registra un nuevo usuario |
| POST | `/api/auth/login` | Inicia sesión y devuelve un token JWT |

#### Registro

```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "Juan Pérez",
  "email": "juan@example.com",
  "password": "MiContraseña123",
  "role": "User"
}
```

#### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "juan@example.com",
  "password": "MiContraseña123"
}
```

Respuesta:

```json
{
  "token": "eyJhbGci...",
  "email": "juan@example.com",
  "role": "User",
  "name": "Juan Pérez"
}
```

### Uso del token

Incluir el token en el header `Authorization` de los endpoints protegidos:

```http
Authorization: Bearer eyJhbGci...
```

## Build

```bash
dotnet build Polla_Futbolera_Backend.slnx
```

## Estructura del proyecto

```
src/
  Domain/           → Entidades, interfaces de repositorio, reglas de negocio
  Application/      → Casos de uso, DTOs, servicios de aplicación
  Infrastructure/   → EF Core, repositorios, JWT, BCrypt
  WebApi/           → Controllers, configuración, DI
```
