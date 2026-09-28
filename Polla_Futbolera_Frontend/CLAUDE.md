# CLAUDE.md — Polla_Futbolera_Frontend

Contexto de desarrollo para Claude Code. Leer antes de implementar cualquier funcionalidad.

## Stack

Angular (última versión) con **standalone components** (sin NgModules) y **signals** para reactividad. Tailwind CSS para estilos.

## Gestión de estado

El estado se gestiona mediante **servicios**, no en los componentes. Hay dos tipos de servicios:

- **`store`**: gestionan el estado de un módulo. Viven dentro del propio módulo (`stores/`). Son quienes llaman a los `api` services.
- **`api`**: encapsulan las llamadas HTTP a un endpoint. Viven en `infrastructure/`, fuera de los módulos (son transversales, no pertenecen a un módulo específico).

### Convención de los store services

- Deben exponer una función **`setup()`** que inicializa el estado inicial del store (llamando a un endpoint del `api` service correspondiente o cargando datos ya disponibles).
- No gestionan el estado de `loading`: eso es responsabilidad del **componente** que consume el store, no del servicio.
- El estado interno se expone mediante `signal`/`computed`, fuertemente tipado con TypeScript.

### Convención de los api services

- Exponen una función **`mapper`** que toma la respuesta cruda del endpoint, valida su estructura con **Yup** y devuelve el dato ya tipado en TypeScript.
- No conocen el estado de la aplicación: solo transforman y devuelven datos.

## Estructura de un módulo

Cuando se indique crear un módulo nuevo, la carpeta `nombre_modulo/` se estructura así:

```
nombre_modulo/
├── components/   # Dumb components (sin estado, reciben props) y Smart components
│                 # (llaman a un store service; en general sin lógica de estado propia,
│                 # solo cuando sea estrictamente necesario)
├── pages/        # Vistas asociadas directamente a una ruta
├── stores/       # Servicios de estado del módulo (llaman al api service correspondiente)
└── routes.ts     # Rutas del módulo (referencian los componentes de pages/)
```

- `routes.ts` de cada módulo se importa y se agrega al router general de la aplicación.
- Los `api` services **no** van dentro del módulo; van en `infrastructure/`.

## Tipado

Todos los datos deben estar fuertemente tipados con TypeScript. No usar `any`. La validación de estructura de las respuestas de API se hace con **Yup** dentro del `mapper` del `api` service correspondiente.

## Loading state

El estado de carga (`loading`) se gestiona en el **componente**, nunca en el store ni en el api service.
