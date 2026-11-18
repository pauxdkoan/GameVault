# GameVault

Plataforma para descubrir videojuegos, llevar tu biblioteca personal, escribir reseñas y crear listas para compartir con otros jugadores. Proyecto de portafolio.

<!--
  CAPTURAS: guarda las imágenes en docs/screenshots/ con estos nombres y borra
  las marcas de comentario de este bloque para que aparezcan en GitHub.

![Inicio](docs/screenshots/home.png)
![Detalle de un juego con reseñas](docs/screenshots/detalle.png)
![Mi biblioteca](docs/screenshots/biblioteca.png)
![Perfil público](docs/screenshots/perfil.png)
-->

## Qué se puede hacer

- **Descubrir juegos:** tendencias, próximos lanzamientos y un catálogo con búsqueda y filtros. Los datos vienen de [IGDB](https://www.igdb.com/).
- **Ver la ficha de un juego:** sinopsis, capturas, vídeos, dónde jugarlo, juegos similares, DLCs y la nota de la comunidad.
- **Mi biblioteca:** guardar juegos como Jugando, Pendiente, Lo quiero, Completado, En pausa o Abandonado, con progreso, horas y nota personal.
- **Reseñas:** nota de 1 a 10 (5 estrellas con medias), aviso de spoilers, me gusta y comentarios.
- **Listas:** colecciones públicas o privadas.
- **Perfil público:** estadísticas, últimas reseñas y listas.
- **Comunidad:** el inicio muestra la actividad reciente y las listas de otros jugadores.

## Tecnologías

| Parte | Tecnología |
|---|---|
| Frontend | Angular 20 (componentes standalone, signals, carga diferida por ruta), Tailwind CSS 4, Swiper, Font Awesome (solo logotipos de plataformas) |
| Backend | .NET 9, ASP.NET Core Web API, MediatR (CQRS), FluentValidation, Entity Framework Core 9 |
| Base de datos | SQL Server |
| Autenticación | ASP.NET Identity + JWT con refresh token |
| Datos de juegos | API de IGDB (Twitch) |
| Pruebas | Jasmine + Karma (frontend), xUnit (backend) |

## Estructura

```
backend/GameVault/
  GameVault/          API (.NET 9)
    Source/Domain/          entidades y enums
    Source/Application/     casos de uso: Feature/<Área>/Commands|Queries
    Source/Infrastructure/  EF Core, migraciones, IGDB, Identity
    Source/WebApi/          controladores y middleware
  GameVault.Tests/    pruebas de los validadores (xUnit)
frontend/GameVault/   Angular
  src/app/gameVault/
    core/       sesión, interceptores, guards, errores HTTP
    shared/     componentes y utilidades reutilizables (iconos, estrellas, avatar...)
    features/   una carpeta por área: auth, games, home, library, reviews, lists, profile
    layouts/    layout público (header + footer) y de autenticación
  docs/CAMBIOS.md   registro detallado de cada cambio del rediseño
```

## Cómo arrancarlo

### Requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express o Developer)
- Node.js 20 o superior
- Una aplicación de Twitch para usar IGDB ([cómo crearla](https://api-docs.igdb.com/#account-creation)): te da un *Client ID* y un *Client Secret*.

### 1. Backend
```bash
cd backend/GameVault/GameVault

# Secretos (no se guardan en el repositorio)
dotnet user-secrets set "IgdbSettings:ClientId" "<tu client id>"
dotnet user-secrets set "IgdbSettings:ClientSecret" "<tu client secret>"
dotnet user-secrets set "JwtSettings:SecretKey" "<una clave larga y aleatoria, 32 caracteres o más>"

# Base de datos: revisa ConnectionStrings:DefaultConnection en appsettings.json
dotnet tool install --global dotnet-ef   # solo la primera vez
dotnet ef database update

dotnet run --launch-profile https
```
La API queda en `https://localhost:7299`, y Swagger en `https://localhost:7299/swagger`. Para probar las rutas privadas en Swagger:
1. Haz login.
2. Pulsa **Authorize**.
3. Pega el `accessToken`.

### 2. Frontend
```bash
cd frontend/GameVault
npm install
npm start
```
Abre `http://localhost:4200`. La URL de la API está en `src/environments/environment.ts`.

### 3. Pruebas
```bash
# Backend
cd backend/GameVault
dotnet test

# Frontend
cd frontend/GameVault
npm test -- --watch=false
```

## API

Las rutas principales son estas. Todas están documentadas en Swagger.

| Ruta | Para qué |
|---|---|
| `POST /api/auth/register` · `login` · `refresh-tokens` · `logout`, `GET /api/auth/me` | Sesión |
| `GET /api/games` · `/trending` · `/upcoming` · `/filters` · `/{id}` | Catálogo y ficha |
| `GET/POST /api/games/{id}/reviews`, `PUT/DELETE /api/reviews/{id}` | Reseñas |
| `POST/DELETE /api/reviews/{id}/like`, `GET/POST /api/reviews/{id}/comments` | Me gusta y comentarios |
| `GET /api/me/library`, `PUT/DELETE /api/me/library/{id}` | Mi biblioteca |
| `GET /api/me/lists`, `GET/POST /api/lists`, `/api/lists/{id}/games/{gameId}` | Listas |
| `GET /api/users/{userName}`, `PUT /api/me/profile` | Perfil |
| `GET /api/activity` | Actividad de la comunidad |

Los errores siempre llegan como [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457), con mensajes en español listos para mostrar.

---
Datos e imágenes de videojuegos proporcionados por [IGDB](https://www.igdb.com/).
