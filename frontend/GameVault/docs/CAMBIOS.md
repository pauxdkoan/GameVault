# Rediseño del frontend de GameVault: registro de cambios

Este documento explica cada cambio del rediseño: qué se hizo, en qué archivo y por qué.
Se irá ampliando con cada paso del plan.

- **Paso 1:** base visual (colores, tipografías y componentes compartidos) ✅
- **Paso 2:** login y register ✅
- **Paso 3:** home ✅
- **Paso 4:** header, footer, catálogo y responsive ✅

Las capturas están en `docs/capturas/`. Se hicieron con datos simulados (imágenes de colores) porque la API no estaba disponible al hacerlas; con la API verás las portadas reales.

**Librerías nuevas:** ninguna. Las tipografías se cargan desde Google Fonts con un `<link>` en `index.html`, sin instalar paquetes.

---

## Paso 1: base visual

### `src/styles.css` (modificado)

Antes solo tenía unos estilos base con colores sueltos (`#090909`, `#061124`, `#071327`...) que se repetían en cada componente.
Ahora es el **sistema de diseño** de la app:

1. **Bloque `@theme`**: en Tailwind 4, cada variable que se declara aquí se convierte automáticamente en clases.
   Por ejemplo, `--color-vault-surface` crea `bg-vault-surface`, `text-vault-surface` y `border-vault-surface`.
   - Fondos: `vault-bg`, `vault-bg-soft`, `vault-surface`, `vault-surface-hover`, `vault-border`
   - Texto: `vault-text` (principal), `vault-muted` (secundario), `vault-subtle` (terciario)
   - Marca: `vault-cyan`, `vault-cyan-soft`, `vault-violet`, `vault-violet-strong` (el cian y violeta del diseño de referencia)
   - Estados: `vault-success`, `vault-warning`, `vault-danger`
   - Tipografías: `font-sans` (Inter) y `font-display` (Space Grotesk, para títulos)
   - Sombras: `shadow-vault-glow` (brillo cian) y `shadow-vault-card`

   **Por qué:** si mañana quieres cambiar el cian por otro color, lo cambias en un solo sitio y se actualiza toda la app.

2. **Utilidades propias con `@utility`**: clases que agrupan varias reglas y se usan como cualquier clase de Tailwind.
   - `bg-vault-gradient`: degradado cian a violeta (botón principal, logo)
   - `text-vault-gradient`: texto con degradado ("en un solo lugar")
   - `vault-card`: tarjeta estándar (borde, fondo, radio, sombra)
   - `vault-glass`: panel tipo cristal translúcido para poner encima de imágenes
   - `vault-container`: contenedor centrado con márgenes responsive
   - `vault-input`: estilo de todos los campos de texto, con estados hover, foco y error (`aria-invalid="true"`)

   **Por qué:** evita repetir cadenas de 15 clases en cada input o tarjeta. El HTML queda más corto y legible.

3. **Estilos base**: los títulos `h1`, `h2` y `h3` usan Space Grotesk automáticamente, el color de selección es de marca y se añadió soporte para `prefers-reduced-motion`, que desactiva las animaciones a quien lo tenga configurado en su sistema (accesibilidad).

### `src/index.html` (modificado)

- `lang="es"`, porque la app está en español (ayuda a lectores de pantalla y al SEO).
- Título más descriptivo, `meta description` y `theme-color`.
- Carga de **Inter** y **Space Grotesk** desde Google Fonts. Antes la app pedía Inter, pero nunca la cargaba, así que se veía la fuente del sistema.

### Componentes compartidos nuevos: `src/app/gameVault/shared/ui/`

Carpeta nueva para **piezas visuales genéricas** que no pertenecen a ninguna feature. Se diferencia de `shared/components`, que tiene componentes con más lógica (header, footer, tarjetas de juego).

| Componente | Archivo | Para qué sirve |
|---|---|---|
| `VaultButton` | `ui/button/vault-button.ts` | Botón con variantes `primary`, `secondary` y `ghost`, y tamaños `sm`, `md` y `lg`. Se usa como **atributo** (`<button vaultButton>` o `<a vaultButton>`), así conserva todo lo nativo: `type="submit"`, `routerLink`, `disabled`... |
| `FormField` | `ui/form-field/form-field.ts` | Etiqueta, input y mensaje de error o ayuda. El `<input>` se pasa como contenido, por eso sigue funcionando con `formControlName` sin lógica extra. |
| `SectionHeader` | `ui/section-header/section-header.ts` | Título, subtítulo y enlace "Ver todos →" de cada sección del home. |
| `BrandLogo` | `ui/brand-logo/brand-logo.ts` | Logo de GameVault, idéntico en el header, el footer y la autenticación. |

---

## Paso 2: login y register

### Diseño

Pantalla dividida en dos:
- **Izquierda:** logo, enlace "Volver al inicio", formulario y pie.
- **Derecha:** un panel con un **mural de portadas** inclinado que se desplaza lentamente y un mensaje de bienvenida en cristal.
  Es la evolución de tus "tarjetas flotantes" originales: se mantiene la idea de enseñar portadas de juegos reales de la API, pero ordenada y con más presencia.
- En móvil y tablet el panel derecho se oculta y el formulario ocupa toda la pantalla.

Ya no se muestran el header ni el footer públicos en estas pantallas. Así el usuario se centra en el formulario, que es lo habitual en plataformas modernas, y se puede volver al inicio desde el logo o el enlace.

### `layouts/auth-layout/` (modificado)

- `auth-layout.ts`: **ya no contiene lógica**. Antes cargaba los juegos y calculaba 20 posiciones a mano; esa lógica pasa al componente `AuthShowcase`. Ahora el layout solo organiza la página.
- `auth-layout.html`: nueva estructura en dos columnas con CSS Grid.
- `auth-layout.css`: **eliminado**. Todo se resuelve con Tailwind y el CSS de la animación vive en `AuthShowcase`.

### Componentes nuevos de la feature: `features/auth/components/`

| Componente | Qué hace |
|---|---|
| `auth-showcase/` | Panel derecho. Pide 18 juegos a la API, reparte las portadas en 4 columnas (`computed`) y las anima con CSS. Si la API falla, pinta bloques de color para que nunca se vea vacío. |
| `auth-form-header/` | Etiqueta pequeña, título y subtítulo encima del formulario. Lo usan login y register. |
| `password-toggle/` | Botón del ojo para mostrar u ocultar la contraseña. Recibe el `<input>` con una referencia de plantilla (`#passwordInput`) y solo cambia su `type`. |
| `form-alert/` | Caja roja con los errores que devuelve la API. Si no hay errores, no se pinta. |

### `features/auth/utils/form-errors.ts` (nuevo)

La función `getControlError(control, mensajes)` devuelve el primer mensaje de error de un campo, solo si el usuario ya lo tocó.

**Por qué:** antes cada campo tenía bloques `@if` anidados en el HTML (`registerForm.get("userName")?.hasError("minlength")`...). Ahora el template solo hace `[error]="fieldError('userName')"` y los textos viven juntos en un objeto, fáciles de encontrar y cambiar.

### `features/auth/services/auth.service.ts` (modificado)

- Nuevo método privado `toMessages()`, que convierte cualquier error HTTP en una lista de textos (`string[]`).
- **Bug corregido:** antes hacía `Object.values(error.error?.errors)`. Si la API estaba caída o el error no traía `errors`, eso fallaba con `TypeError` y el usuario no veía ningún mensaje. Ahora muestra "No pudimos conectar con el servidor".
- También aplana los errores por campo de la API (`{ Email: ["..."] }`) para mostrar el texto en lugar de un array.
- Errata corregida: `regiserRequest` pasa a `registerRequest`.

### `features/auth/pages/login-page/` (modificado)

- Diseño nuevo con `FormField`, `PasswordToggle`, `FormAlert` y `VaultButton`.
- Se quitó el comentario "From Uiverse.io": el formulario ahora es propio.
- `fb.nonNullable.group(...)`: formulario tipado, así `getRawValue()` ya tiene la forma de `LoginRequest`.
- Al pulsar "Entrar" con campos vacíos se marcan todos como tocados (`markAllAsTouched`), para que el usuario vea qué falta. Antes el botón no hacía nada.
- Spinner y texto "Entrando..." mientras se envía; el botón se desactiva para evitar doble envío.
- `autocomplete` correcto en cada campo, para que el navegador y los gestores de contraseñas lo rellenen.
- **Bugs corregidos:**
  - El enlace "Signup" apuntaba a `href="#"`; ahora lleva a `/auth/register`.
  - `isLoading` nunca volvía a `false` si el login fallaba, y el `setTimeout` de 1 segundo sobraba.
  - El mensaje de error (`isError`) se guardaba, pero nunca se mostraba en el HTML.
- `(submit)` pasa a `(ngSubmit)`, la forma recomendada en formularios reactivos.

### `features/auth/pages/register-page/` (modificado)

- Mismo diseño y componentes que el login. Nombre y usuario van en dos columnas en escritorio.
- El validador de contraseñas (`matchPasswords`) sale como función pura fuera de la clase y **ya no borra otros errores** del campo de confirmar contraseña (antes `setErrors(null)` podía quitar también el error `required`).
- **Bugs corregidos:**
  - El título decía "Login".
  - Los errores de la API se mostraban como arrays (`[object Object]`).
  - Se quitó el `console.log` de la respuesta.
- Después de registrarse se envía al usuario a **iniciar sesión**, porque el registro de la API no devuelve tokens. Antes lo mandaba al home sin sesión.

### `features/auth/models/login.interfaces.ts` (modificado)

`BackgroundGame` (con posiciones, rotación y escala) se sustituye por `ShowcaseCover` (`id`, `name` y `cover`), porque el nuevo mural ya no necesita coordenadas.

### Pendiente detectado (no tocado)

- Al hacer login, **los tokens no se guardan** en ningún sitio; el login navega al home, pero la app no sabe que hay sesión. Habrá que añadir un servicio de sesión cuando se trabaje esa lógica.
- No hay "¿Olvidaste tu contraseña?" ni login con Google o Steam, porque el backend aún no lo soporta. Se puede añadir el enlace cuando exista.

---

## Paso 3: home

### Estructura nueva

El home sigue el diseño de referencia (`docs/designs/Normal.png`). Cada sección es **su propio componente** dentro de `features/home/components/`, y la página solo reparte los datos:

| Orden | Sección | Componente | Datos |
|---|---|---|---|
| 1 | Hero: "Tu universo de videojuegos, en un solo lugar" + Juego del momento | `hero-section` + `hero-carousel` | API (tendencias y lanzamientos) |
| 2 | Tendencias esta semana (6 tarjetas) | `home-game-section` | API |
| 3 | Próximos lanzamientos (filas horizontales) | `home-upcoming-list` (nuevo) | API |
| 4 | Mucho más que un catálogo (4 tarjetas) | `home-features` (nuevo) | Fijos |
| 5 | Tu biblioteca. Tus reglas. (maqueta) | `home-library-preview` (nuevo) | Portadas de la API + estados de ejemplo |
| 6 | Descubre lo que juega la comunidad | `home-community` (nuevo) | **Ejemplo** |
| 7 | Listas creadas por jugadores | `home-player-lists` (nuevo) | Títulos de **ejemplo** + portadas de la API |
| 8 | Llamada final "Empieza a construir tu historia" | `home-cta` (nuevo) | Fijos |

### `features/home/data/home-sample-data.ts` (nuevo)

Aquí están **todos los datos de ejemplo** juntos: features, actividad de la comunidad y listas.
El backend todavía no tiene reseñas, actividad ni listas. Cuando existan, solo hay que cambiar estas constantes por una llamada en `HomeDataService`, sin tocar los componentes.

### `pages/public-home/home-page-public.ts` y `.html` (modificados)

- Importa las 8 secciones y les pasa los datos con inputs.
- Las secciones con datos de la API (tendencias y lanzamientos) muestran un **esqueleto** mientras cargan y un aviso con "Reintentar" si fallan.
- Las secciones informativas se ven **siempre**, aunque la API falle, para que el home nunca quede vacío.
- Se quitó el estado `'empty'` y el `computed` `gameSections`: ahora cada sección se pinta con su propio `@if`, que es más fácil de seguir.

### `hero-section` (modificado)

- Textos nuevos según el diseño, insignia "Nueva temporada de lanzamientos", título con degradado y dos botones (`VaultButton`).
- Fondo: la imagen del primer juego destacado, muy difuminada, más brillos cian y violeta.
- **Bug corregido:** el botón "Explorar catálogo" llevaba a `/games`, que no existe (la ruta real es `/gamevault/games`), así que el comodín `**` te devolvía al home.
- **Bug corregido (móvil):** el carrusel dentro de un grid sin `min-w-0` hacía que Swiper calculara un ancho infinito y la tarjeta ocupaba millones de píxeles en pantallas pequeñas. Se añadieron `grid-cols-1` y `min-w-0`.

### `hero-carousel` (modificado)

- Tarjeta vertical (4:5) con la información en un panel de cristal: etiqueta "Juego del momento", nombre, nota, géneros, fecha y botón **"Ver detalles"**, que lleva al detalle del juego (antes no había enlace).
- La nota se muestra con un decimal (`toFixed(1)`).

### `home-game-section` (modificado)

- Ahora recibe `title`, `subtitle`, `games` y `maxGames` por separado, en lugar de un objeto `section`. Así se ve qué necesita sin abrir el modelo.
- Usa `SectionHeader`, con el enlace "Ver todos los juegos →".
- Grid de 2, 3 o 6 columnas según la pantalla. Las dos últimas tarjetas abren su vista previa hacia la izquierda para que no se salga de la pantalla.

### `models/home-content.model.ts` (modificado)

- Se eliminó `HomeGameSection` (ya no se usa).
- Se añadieron `HomeFeature`, `CommunityActivity` y `PlayerList` para tipar los datos de ejemplo.

### `services/home-data.service.ts` (modificado)

- **Bug corregido:** el `catchError` estaba escrito **dentro** de `map(...)` como segundo argumento, así que nunca se ejecutaba. Ahora va como operador propio dentro de `pipe`.
- El mensaje de error tenía `'${{ error }}'`, que imprimía `[object Object]`; ahora es un texto claro.

### Decisiones

- "Seguir lanzamiento" del diseño se cambió por **"Ver detalles"**, porque seguir juegos necesita backend. Cuando exista, el botón se añade en `home-upcoming-list`.
- Las tarjetas de listas muestran "Explorar lista →" como texto, no como enlace, porque aún no hay página de listas.

---

## Paso 4: header, footer, catálogo y detalle

### `shared/components/public-header/` (modificado)

- Usa `BrandLogo` y `VaultButton`. Fondo translúcido con desenfoque al hacer scroll.
- Navegación: **Inicio · Explorar ▾ · Juegos · Comunidad**.
- **Bugs corregidos en los enlaces de "Explorar":**
  - "Todos los juegos" apuntaba a `games` (ruta relativa); ahora apunta a `/gamevault/games`.
  - "Tendencias" tenía el fragmento `'/trending'` con una barra de más, así que no hacía scroll.
  - "Descubre más" apuntaba a una sección que ya no existe; se sustituye por "Listas de jugadores".
- "Iniciar sesión" y "Registrarse" eran `<button routerLink>`; ahora son `<a vaultButton>`, porque un enlace que navega debe ser `<a>` (accesibilidad y clic derecho, "abrir en pestaña nueva"). También se quitó la opacidad que los hacía parecer desactivados.
- El menú móvil se rediseñó con los mismos colores y botones.

### `shared/components/public-footer/` (modificado)

- Columnas definidas en un único array (`columns`). Para añadir un enlace solo se edita la lista.
- Enlaces corregidos: antes apuntaban a `/` con fragmentos, y `/` redirige, así que se perdía el fragmento.
- El año se calcula solo (`currentYear`).
- Se quitaron los textos internos ("datos mock solo como respaldo temporal", "canales sociales pendientes de configurar") y se añadió el crédito a IGDB, que es de donde vienen los datos.

### `layouts/public-layout/public-layout.html` (modificado)

Fondo `#090909` sustituido por el token `bg-vault-bg`.

### Catálogo, detalle y tarjetas de juego (solo colores)

En `features/games/**` y `shared/components/game-card*` **no se cambió la estructura ni la lógica**, solo los colores fijos, para que vayan a juego con el resto:

| Antes | Ahora |
|---|---|
| `bg-[#071327]` | `bg-vault-surface` |
| `bg-[#07152c]` | `bg-vault-bg-soft` |
| `bg-[#061124]` | `bg-vault-bg` |
| `text-cyan-100/xx` | `text-vault-muted` |
| `text-cyan-200` / `text-cyan-300` | `text-vault-cyan-soft` / `text-vault-cyan` |
| `border-cyan-200/300` | `border-vault-cyan` |
| `bg-cyan-300/400` | `bg-vault-cyan` |
| Nota de la tarjeta en azul (`bg-blue-500`) | Degradado de marca |
| Botón "Buscar" y filtros activos en amarillo (`amber`) | Degradado y cian de marca |

Las páginas de detalle y catálogo se pueden pulir más a fondo en una siguiente fase, ya que no estaban entre las vistas que te desagradaban.

### Nota sobre el tamaño del bundle

`ng build` avisa de que el bundle inicial supera el presupuesto de 500 kB. **El aviso ya existía antes:** la build anterior pesaba unos 780 kB, sobre todo por Swiper y Font Awesome. Con este rediseño pasa a unos 840 kB en total; la parte de JavaScript sube solo unos 30 kB. Se puede reducir más adelante con carga diferida (`loadComponent`) en las rutas.
