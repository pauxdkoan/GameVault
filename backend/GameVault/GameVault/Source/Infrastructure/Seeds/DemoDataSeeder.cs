using GameVault.Source.Application.Enums;
using GameVault.Source.Application.Interfaces.Games;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Domain.Enums;
using GameVault.Source.Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Infrastructure.Seeds
{
    /// <summary>
    /// Datos de ejemplo para que el portafolio no se vea vacío: 20 usuarios con
    /// biblioteca, reseñas, "me gusta", comentarios y listas.
    ///
    /// - No cambia el esquema de la base de datos: no hace falta migración.
    /// - Solo se ejecuta una vez: si ya existe algún usuario de ejemplo
    ///   (email @demo.gamevault.dev), no hace nada.
    /// - Los juegos no están escritos a mano: se toman de IGDB al arrancar
    ///   (tendencias + populares) y se guardan con GameSyncService, igual que
    ///   cuando un usuario real añade un juego. Así los ids siempre son válidos.
    /// - Usa un Random con semilla fija: cada base de datos nueva recibe los mismos datos.
    /// - Se desactiva con la configuración SeedSettings:DemoData = false.
    /// </summary>
    public sealed class DemoDataSeeder
    {
        public const string DemoEmailDomain = "demo.gamevault.dev";

        private const int GamesToUse = 24;

        private readonly GameVaultContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIgdbApiClient _igdbApiClient;
        private readonly IGameSyncService _gameSyncService;
        private readonly ILogger<DemoDataSeeder> _logger;
        private readonly Random _random = new(2026);
        private readonly DateTime _now = DateTime.UtcNow;

        public DemoDataSeeder(
            GameVaultContext context,
            UserManager<ApplicationUser> userManager,
            IIgdbApiClient igdbApiClient,
            IGameSyncService gameSyncService,
            ILogger<DemoDataSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _igdbApiClient = igdbApiClient;
            _gameSyncService = gameSyncService;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            // Ya hecho si algún usuario de ejemplo tiene biblioteca. (Si un arranque
            // anterior creó los usuarios pero falló después, se reutilizan y se completa.)
            var alreadySeeded = await _context.UserGames
                .AnyAsync(entry => entry.ApplicationUser.Email!.EndsWith("@" + DemoEmailDomain), cancellationToken);

            if (alreadySeeded)
            {
                return;
            }

            // 1. Juegos: si IGDB falla no se crea nada y se vuelve a intentar en el próximo arranque.
            var games = await LoadGamesAsync(cancellationToken);
            if (games.Count < 8)
            {
                _logger.LogWarning("Datos de ejemplo: IGDB devolvió pocos juegos ({Count}); se omite.", games.Count);
                return;
            }

            // 2. Usuarios
            var users = await CreateUsersAsync();
            if (users.Count == 0)
            {
                return;
            }

            // 3. Biblioteca, reseñas, likes, comentarios y listas
            var libraries = CreateLibraries(users, games);
            var reviews = CreateReviews(users, libraries);
            CreateLikes(users, reviews);
            CreateComments(users, reviews);
            CreateLists(users, games);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Datos de ejemplo creados: {Users} usuarios, {Games} juegos, {Reviews} reseñas.",
                users.Count, games.Count, reviews.Count);
        }

        // ---------------------------------------------------------------- juegos

        private async Task<List<Game>> LoadGamesAsync(CancellationToken cancellationToken)
        {
            var trending = await _igdbApiClient.GetTrendingGamesAsync(1, 20, cancellationToken);
            var popular = await _igdbApiClient.GetGamesAsync(
                null, null, null, GameSort.Popular, 1, 20, cancellationToken);

            var externalIds = popular.Results
                .Concat(trending.Results)
                .Select(game => game.Id)
                .Where(id => id > 0)
                .Distinct()
                .Take(GamesToUse)
                .ToList();

            var games = new List<Game>();
            foreach (var externalId in externalIds)
            {
                try
                {
                    games.Add(await _gameSyncService.EnsureGameAsync(externalId, cancellationToken));
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogWarning(exception, "Datos de ejemplo: no se pudo guardar el juego {Id}.", externalId);
                }
            }

            return games;
        }

        // -------------------------------------------------------------- usuarios

        private async Task<List<ApplicationUser>> CreateUsersAsync()
        {
            var users = new List<ApplicationUser>();

            foreach (var profile in Profiles)
            {
                var existing = await _userManager.FindByNameAsync(profile.UserName);
                if (existing is not null)
                {
                    users.Add(existing);
                    continue;
                }

                var user = new ApplicationUser
                {
                    UserName = profile.UserName,
                    Email = $"{profile.UserName}@{DemoEmailDomain}",
                    EmailConfirmed = true,
                    DisplayName = profile.DisplayName,
                    Bio = profile.Bio,
                    Country = profile.Country,
                    IsActive = true,
                    CreatedAt = _now.AddDays(-_random.Next(60, 400)),
                };

                // Nadie inicia sesión con estas cuentas: la contraseña es aleatoria y no se guarda.
                var password = $"Demo-{Guid.NewGuid():N}-Aa1!";
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning(
                        "Datos de ejemplo: no se pudo crear {User}: {Errors}",
                        profile.UserName,
                        string.Join(", ", result.Errors.Select(error => error.Description)));
                    continue;
                }

                users.Add(user);
            }

            return users;
        }

        // ------------------------------------------------------------ biblioteca

        private Dictionary<ApplicationUser, List<UserGame>> CreateLibraries(
            List<ApplicationUser> users,
            List<Game> games)
        {
            var libraries = new Dictionary<ApplicationUser, List<UserGame>>();

            foreach (var user in users)
            {
                var entries = new List<UserGame>();

                foreach (var game in Pick(games, _random.Next(6, 13)))
                {
                    var status = PickStatus(game);
                    var added = RandomDateAfter(user.CreatedAt ?? _now.AddDays(-200));
                    var hours = status is GameStatus.Wishlist ? (decimal?)null : _random.Next(2, 140);

                    var entry = new UserGame
                    {
                        ApplicationUserId = user.Id,
                        GameId = game.Id,
                        Status = status,
                        CreatedAt = added,
                        UpdatedAt = RandomDateAfter(added),
                        HoursPlayed = hours,
                        ProgressPercentage = status switch
                        {
                            GameStatus.Completed => 100,
                            GameStatus.Playing => _random.Next(10, 90),
                            GameStatus.Paused => _random.Next(20, 70),
                            GameStatus.Dropped => _random.Next(5, 40),
                            _ => 0
                        },
                        PersonalRating = status is GameStatus.Completed or GameStatus.Playing or GameStatus.Dropped
                            ? RatingFor(game, status)
                            : null,
                        StartedAt = status is GameStatus.Wishlist or GameStatus.Backlog
                            ? null
                            : DateOnly.FromDateTime(added),
                        CompletedAt = status is GameStatus.Completed
                            ? DateOnly.FromDateTime(RandomDateAfter(added))
                            : null,
                        LastPlayedAt = status is GameStatus.Playing ? _now.AddDays(-_random.Next(0, 6)) : null,
                    };

                    entries.Add(entry);
                    _context.UserGames.Add(entry);
                }

                libraries[user] = entries;
            }

            return libraries;
        }

        private GameStatus PickStatus(Game game)
        {
            // Los juegos que aún no salieron solo pueden estar en "Lo quiero".
            if (game.ReleaseDate is { } release && release > DateOnly.FromDateTime(_now))
            {
                return GameStatus.Wishlist;
            }

            var roll = _random.Next(100);
            return roll switch
            {
                < 30 => GameStatus.Completed,
                < 52 => GameStatus.Playing,
                < 68 => GameStatus.Wishlist,
                < 82 => GameStatus.Backlog,
                < 92 => GameStatus.Paused,
                _ => GameStatus.Dropped
            };
        }

        /// <summary>Nota 1-10 cercana a la de IGDB (0-5 → x2) para que las reseñas tengan sentido.</summary>
        private int RatingFor(Game game, GameStatus status)
        {
            var baseRating = game.ExternalRating is { } rating and > 0 ? (double)rating * 2 : 7.5;
            var noise = _random.Next(-2, 2);
            var penalty = status is GameStatus.Dropped ? -3 : 0;
            return Math.Clamp((int)Math.Round(baseRating) + noise + penalty, 1, 10);
        }

        // -------------------------------------------------------------- reseñas

        private List<Review> CreateReviews(
            List<ApplicationUser> users,
            Dictionary<ApplicationUser, List<UserGame>> libraries)
        {
            var reviews = new List<Review>();

            foreach (var user in users)
            {
                var playable = libraries[user]
                    .Where(entry => entry.PersonalRating is not null)
                    .ToList();

                foreach (var entry in Pick(playable, Math.Min(playable.Count, _random.Next(2, 5))))
                {
                    var rating = entry.PersonalRating!.Value;
                    var (title, content) = ReviewText(rating);

                    var review = new Review
                    {
                        ApplicationUserId = user.Id,
                        GameId = entry.GameId,
                        Rating = rating,
                        Title = title,
                        Content = content,
                        ContainsSpoilers = _random.Next(100) < 12,
                        IsVisible = true,
                        CreatedAt = RandomDateAfter(entry.CreatedAt),
                    };

                    reviews.Add(review);
                    _context.Reviews.Add(review);
                }
            }

            return reviews;
        }

        private (string Title, string Content) ReviewText(int rating)
        {
            var pool = rating switch
            {
                >= 9 => GreatReviews,
                >= 7 => GoodReviews,
                >= 5 => MixedReviews,
                _ => BadReviews
            };

            return pool[_random.Next(pool.Length)];
        }

        private void CreateLikes(List<ApplicationUser> users, List<Review> reviews)
        {
            foreach (var review in reviews)
            {
                var others = users.Where(user => user.Id != review.ApplicationUserId).ToList();

                foreach (var user in Pick(others, _random.Next(0, 9)))
                {
                    _context.ReviewLikes.Add(new ReviewLike
                    {
                        ReviewId = review.Id,
                        ApplicationUserId = user.Id,
                        CreatedAt = RandomDateAfter(review.CreatedAt),
                    });
                }
            }
        }

        private void CreateComments(List<ApplicationUser> users, List<Review> reviews)
        {
            foreach (var review in reviews)
            {
                var others = users.Where(user => user.Id != review.ApplicationUserId).ToList();
                var comments = review.Rating >= 7 ? PositiveComments : CriticalComments;

                foreach (var user in Pick(others, _random.Next(0, 4)))
                {
                    _context.ReviewComments.Add(new ReviewComment
                    {
                        ReviewId = review.Id,
                        ApplicationUserId = user.Id,
                        Content = comments[_random.Next(comments.Length)],
                        CreatedAt = RandomDateAfter(review.CreatedAt),
                    });
                }
            }
        }

        // --------------------------------------------------------------- listas

        private void CreateLists(List<ApplicationUser> users, List<Game> games)
        {
            foreach (var user in users)
            {
                foreach (var template in Pick(ListTemplates, _random.Next(1, 3)))
                {
                    var list = new GameList
                    {
                        ApplicationUserId = user.Id,
                        Name = template.Name,
                        Description = template.Description,
                        IsPublic = _random.Next(100) < 85,
                        CreatedAt = RandomDateAfter(user.CreatedAt ?? _now.AddDays(-200)),
                    };

                    var position = 1;
                    foreach (var game in Pick(games, _random.Next(4, 9)))
                    {
                        list.Items.Add(new GameListItem
                        {
                            GameId = game.Id,
                            Position = position++,
                            Note = _random.Next(100) < 30 ? ListNotes[_random.Next(ListNotes.Length)] : null,
                        });
                    }

                    _context.GameLists.Add(list);
                }
            }
        }

        // -------------------------------------------------------------- ayudas

        private List<T> Pick<T>(IReadOnlyList<T> source, int count)
        {
            return source
                .OrderBy(_ => _random.Next())
                .Take(Math.Clamp(count, 0, source.Count))
                .ToList();
        }

        /// <summary>Una fecha entre "from" y ahora (nunca en el futuro).</summary>
        private DateTime RandomDateAfter(DateTime from)
        {
            var span = _now - from;
            if (span <= TimeSpan.Zero)
            {
                return _now;
            }

            return from.AddMinutes(_random.NextDouble() * span.TotalMinutes);
        }

        // ---------------------------------------------------------------- textos

        private sealed record DemoProfile(string UserName, string DisplayName, string Country, string Bio);

        private sealed record ListTemplate(string Name, string Description);

        private static readonly DemoProfile[] Profiles =
        [
            new("lucia_rpg", "Lucía Fernández", "España", "RPG de 80 horas, mapas llenos de iconos y café. Siempre con un JRPG empezado."),
            new("mateo.speed", "Mateo Rojas", "Chile", "Speedrunner a ratos. Si tiene buen movimiento, lo juego dos veces."),
            new("valen_indie", "Valentina Cruz", "Argentina", "Indies primero, AAA después. Me encantan los juegos cortos que te dejan pensando."),
            new("santi_souls", "Santiago Pérez", "Colombia", "Morir 200 veces al mismo jefe es mi forma de relajarme."),
            new("cami.cozy", "Camila Torres", "México", "Granjas, pueblitos y juegos sin prisa. Pocas cosas me gustan más que un buen cozy game."),
            new("diego_fps", "Diego Herrera", "Perú", "Shooters competitivos de noche, campañas de un jugador los domingos."),
            new("sofi_lore", "Sofía Ramírez", "Uruguay", "Leo todos los textos del juego. Todos. Teorías de lore bienvenidas."),
            new("nico.retro", "Nicolás Silva", "Bolivia", "Crecí con la SNES y sigo buscando esa misma sensación en juegos nuevos."),
            new("ana_coop", "Ana Morales", "Ecuador", "Si se puede jugar en cooperativo, ahí estoy. Mejor con amigos."),
            new("joaquin_strat", "Joaquín Vargas", "España", "Estrategia por turnos y partidas de 'un turno más' hasta las 3 de la mañana."),
            new("isa.platformer", "Isabella Castro", "Costa Rica", "Plataformas exigentes y metroidvanias. Coleccionista de 100%."),
            new("tomas_open", "Tomás Gutiérrez", "Paraguay", "Mundos abiertos para perderme. Ignoro la misión principal durante horas."),
            new("emilia_story", "Emilia Navarro", "España", "Busco historias que me hagan llorar. Si tiene buena banda sonora, mejor."),
            new("martin.horror", "Martín Ortega", "Argentina", "Terror y supervivencia con las luces apagadas. Sí, me asusto igual."),
            new("renata_sports", "Renata Medina", "México", "Fútbol, carreras y cualquier cosa con modo carrera largo."),
            new("benja_rogue", "Benjamín Reyes", "Chile", "Roguelikes: una partida más y lo dejo. (Nunca lo dejo)."),
            new("julia.switch", "Julia Romero", "Venezuela", "Juego casi todo en portátil. El bus es mi sala de juegos."),
            new("gabo_mmo", "Gabriel Suárez", "Colombia", "Ex-raid leader. Ahora juego tranquilo y comparto guías."),
            new("paula_puzzle", "Paula Jiménez", "Guatemala", "Puzzles, misterios y juegos que me hagan sentir lista."),
            new("leo.casual", "Leonardo Díaz", "Panamá", "Juego un poco de todo, sobre todo lo que recomienda la comunidad."),
        ];

        private static readonly (string, string)[] GreatReviews =
        [
            ("Una obra maestra", "Pocas veces un juego me atrapa así desde la primera hora. Todo encaja: jugabilidad, ritmo, música y dirección de arte. Lo terminé y quería volver a empezar."),
            ("Imprescindible", "Si tienes que jugar un solo juego este año, que sea este. Tiene detalles que se notan hechos con cariño y nunca se siente repetitivo."),
            ("Me voló la cabeza", "Empecé con dudas y acabé jugándolo cada noche. Los últimos capítulos son de lo mejor que he jugado en años."),
            ("De mis favoritos de siempre", "Mundo precioso, controles perfectos y una historia que no te suelta. Ya lo recomendé a todos mis amigos."),
        ];

        private static readonly (string, string)[] GoodReviews =
        [
            ("Muy recomendable", "Muy buen juego con algunos altibajos. La parte central se alarga un poco, pero el final compensa y la jugabilidad se siente genial."),
            ("Sólido y divertido", "No reinventa nada, pero todo lo que hace lo hace bien. Ideal para jugar a ratos y disfrutarlo sin complicaciones."),
            ("Mejor de lo que esperaba", "Llegué sin expectativas y me encontré un juego pulido y con mucha personalidad. Algún bug menor, nada grave."),
            ("Gran experiencia", "Lo disfruté mucho, sobre todo el apartado artístico. Le falta un poco de variedad en las misiones secundarias."),
        ];

        private static readonly (string, string)[] MixedReviews =
        [
            ("Tiene cosas buenas, pero...", "La idea es buena y hay momentos brillantes, pero se hace repetitivo pronto. Lo recomendaría solo en oferta."),
            ("Irregular", "Empieza muy fuerte y luego pierde fuelle. Me quedo con la ambientación; la historia no me terminó de enganchar."),
            ("Correcto sin más", "Cumple, pero no destaca en nada. Se deja jugar y poco más."),
        ];

        private static readonly (string, string)[] BadReviews =
        [
            ("No es para mí", "Lo intenté varias veces, pero no conecté con su propuesta. El control se siente pesado y el ritmo es muy lento."),
            ("Decepcionante", "Esperaba mucho más. Problemas técnicos, misiones de relleno y una historia que no va a ninguna parte."),
        ];

        private static readonly string[] PositiveComments =
        [
            "Totalmente de acuerdo, ¡qué final!",
            "Me convenciste, lo pongo en mi lista.",
            "Gran reseña. A mí me pasó lo mismo con la banda sonora.",
            "¿Cuántas horas te llevó terminarlo?",
            "Lo estoy jugando ahora y voy por el mismo camino.",
            "Por fin alguien que lo valora como merece.",
        ];

        private static readonly string[] CriticalComments =
        [
            "Yo lo disfruté más, pero entiendo lo que dices.",
            "Dale otra oportunidad, la segunda mitad mejora mucho.",
            "Coincido, se hace muy largo para lo que cuenta.",
            "Justo por esto lo dejé a la mitad.",
            "Buena reseña, muy honesta.",
        ];

        private static readonly ListTemplate[] ListTemplates =
        [
            new("Para empezar en los videojuegos", "Juegos accesibles para recomendar a quien empieza."),
            new("Joyas para el fin de semana", "Se terminan en pocas sesiones y dejan huella."),
            new("Mis favoritos de todos los tiempos", "Los que volvería a jugar mañana mismo."),
            new("Pendientes de este año", "Lo que tengo en el radar para los próximos meses."),
            new("Para jugar con amigos", "Cooperativo, risas y caos asegurado."),
            new("Bandas sonoras increíbles", "Juegos que también escucho sin jugar."),
            new("Mundos para perderse", "Exploración, secretos y horas que se van volando."),
            new("Retos de verdad", "Para cuando quiero sufrir un poco."),
        ];

        private static readonly string[] ListNotes =
        [
            "Empieza por aquí.",
            "El mejor del género para mí.",
            "Mejor jugarlo con auriculares.",
            "Corto pero intenso.",
            "Paciencia con las primeras horas.",
        ];
    }
}
