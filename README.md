# Polla Futbolera

Monorepo con la API (.NET 10) en `Polla_Futbolera_Backend/` y el frontend (Angular) en `Polla_Futbolera_Frontend/`. Esta guía cubre únicamente la ejecución con Docker.

## Requisitos

- [Docker](https://www.docker.com/get-started) con Docker Compose

## Levantar el stack completo

Desde la raíz del repositorio:

```bash
docker compose up --build
```

Esto construye y levanta tres servicios:

| Servicio | URL | Descripción |
|----------|-----|-------------|
| `frontend` | http://localhost:4200 | Aplicación Angular |
| `api` | http://localhost:8080 | API REST (.NET) |
| `api` (Scalar) | http://localhost:8080/scalar/v1 | Documentación interactiva de la API |
| `db` | localhost:5432 | PostgreSQL |

Para levantarlo en segundo plano:

```bash
docker compose up --build -d
```

### Credenciales de PostgreSQL

Definidas en `Polla_Futbolera_Backend/docker-compose.yml`:

| Variable | Valor |
|----------|-------|
| `POSTGRES_USER` | `postgres` |
| `POSTGRES_PASSWORD` | `yourpassword` |
| `POSTGRES_DB` | `PolleFutboleraDb` |

### Usuarios de prueba (seed)

Se insertan automáticamente vía EF Core `HasData` (`Polla_Futbolera_Backend/src/Infrastructure/Persistence/Configurations/UserConfiguration.cs`) al aplicar las migraciones. Todos comparten la misma contraseña:

| Nombre | Email | Password | Rol |
|--------|-------|----------|-----|
| Luisa Perez | `admin@admin.com` | `Password123!` | Admin |
| Juan Díaz | `player1@player.com` | `Password123!` | Player |
| Mario Torres | `player@player.com` | `Password123!` | Player |

Úsalos contra `POST /api/auth/login` desde Scalar o el frontend para obtener el token JWT.

## Migraciones y base de datos

Con los contenedores levantados, las migraciones se ejecutan dentro del contenedor `api`:

```bash
# Crear una migración
docker compose exec api dotnet ef migrations add <NombreMigracion> \
  --project src/Infrastructure \
  --startup-project src/WebApi

# Aplicar migraciones a la base de datos
docker compose exec api dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/WebApi
```

La base de datos (`PolleFutboleraDb`) se crea automáticamente en el contenedor `db` al levantar el stack, usando las credenciales definidas en `Polla_Futbolera_Backend/docker-compose.yml`. El servicio `api` espera a que `db` esté saludable (`healthcheck`) antes de iniciar.

Para conectarte directamente a la base de datos desde el host:

```bash
docker compose exec db psql -U postgres -d PolleFutboleraDb
```

## Detener los contenedores

```bash
docker compose down        # detiene y elimina los contenedores
docker compose down -v     # además elimina el volumen de PostgreSQL (borra los datos)
```

## Arquitectura

### Backend — Clean Architecture (.NET 10)

Cuatro proyectos con dependencias en una sola dirección:

```
Domain ← Application ← Infrastructure ← WebApi
```

| Proyecto | Contiene |
|----------|----------|
| `Domain` | Entidades (`Bet`, `Team`, `User`, `Match`), enums, excepciones, interfaces de repositorio — sin dependencias externas |
| `Application` | Servicios de caso de uso (`AuthService`, `BetService`, `MatchService`, `LeaderboardService`) y DTOs |
| `Infrastructure` | `AppDbContext`, configuraciones EF Core, repositorios, hashing de contraseñas (BCrypt), migraciones |
| `WebApi` | Controllers (`Auth`, `Matches`, `AdminMatches`, `Bets`, `Leaderboard`), `Program.cs`, JWT, Scalar |
| `Test` | Tests de controllers, dominio y servicios |

Detalle completo (convenciones de código, comandos de migraciones, estructura extendida): [`Polla_Futbolera_Backend/CLAUDE.md`](Polla_Futbolera_Backend/CLAUDE.md)

### Frontend — Angular (standalone + signals)

Componentes standalone (sin NgModules), estado con `signal`/`computed`, Tailwind CSS. Cada módulo de negocio sigue la misma estructura:

```
modulo/
├── components/   # componentes de UI
├── pages/        # vistas de ruta
├── stores/       # estado del módulo (signals + llamadas a servicios api)
└── routes.ts     # rutas del módulo
```

Módulos en `src/app/modules/`:

| Módulo | Contenido |
|--------|-----------|
| `auth` | Login, registro, input de contraseña, store de sesión |
| `player` | Apuestas por partido, tabla de posiciones, historial de usuario |
| `admin` | Gestión de partidos (fila editable, listado) |

Las llamadas HTTP viven fuera de `modules/`, en `infrastructure/api/` (un servicio `*-api.service.ts` + tipos por dominio: auth, bets, leaderboard, matches), validadas con Yup.

Detalle completo (convenciones de stores/api, estructura de carpetas): [`Polla_Futbolera_Frontend/CLAUDE.md`](Polla_Futbolera_Frontend/CLAUDE.md)

## Más detalles

- Backend (arquitectura, endpoints, ejecución local): [`Polla_Futbolera_Backend/README.md`](Polla_Futbolera_Backend/README.md)
- Frontend: [`Polla_Futbolera_Frontend/README.md`](Polla_Futbolera_Frontend/README.md)
