# GameVault · frontend

Angular 20 + Tailwind CSS 4. Las instrucciones completas (backend, base de datos, secretos) están en el [README principal](../../README.md).

## Comandos
| Comando | Qué hace |
|---|---|
| `npm start` | Servidor de desarrollo en `http://localhost:4200` |
| `npm run build` | Compilación de producción en `dist/` |
| `npm test -- --watch=false` | Pruebas unitarias (Jasmine + Karma) |

La URL de la API está en `src/environments/environment.ts` (por defecto `https://localhost:7299/api`).

## Organización (`src/app/gameVault/`)
- `core/`: sesión (`AuthSessionService`), interceptores HTTP, guards y `ApiError`.
- `shared/`: piezas reutilizables.
  - `ui/`: botón, icono, estrellas, avatar, insignia de plataforma, cabeceras.
  - `components/`: header, footer, tarjetas de juego.
  - `mappers/`, `models/` y `utils/`.
- `features/<área>/`: `models/` (tipos de la API), `services/` (HTTP), `components/` y `pages/` (una por ruta).
- `layouts/`: layout público y layout de login y registro.

Cada página se carga solo cuando se visita (`loadComponent` en `app.routes.ts`).

## Iconos
- Iconos de interfaz: `<app-icon name="heart" class="size-4" />`. Todos están en `shared/ui/icon/icons.ts`.
- Logotipos de plataformas: `<app-platform-badge family="PlayStation" />`. Usa Font Awesome y se configura en `shared/ui/platform-badge/platform-families.ts`.

Todos los cambios del rediseño están explicados en [`docs/CAMBIOS.md`](docs/CAMBIOS.md).
