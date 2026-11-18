using GameVault.Source.Application.Dtos.Igdb;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Games;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Services.Games
{
    /// <summary>
    /// Guarda en local los juegos de IGDB que usan los usuarios.
    /// Solo se guardan los juegos que alguien añade a su biblioteca, reseña o lista,
    /// no todo el catálogo de IGDB.
    /// </summary>
    public sealed class GameSyncService : IGameSyncService
    {
        private readonly GameVaultContext _context;
        private readonly IIgdbApiClient _igdbApiClient;

        public GameSyncService(GameVaultContext context, IIgdbApiClient igdbApiClient)
        {
            _context = context;
            _igdbApiClient = igdbApiClient;
        }

        public async Task<Game> EnsureGameAsync(int externalId, CancellationToken cancellationToken)
        {
            var existing = await _context.Games
                .FirstOrDefaultAsync(g => g.ExternalId == externalId, cancellationToken);

            if (existing is not null)
            {
                return existing;
            }

            var igdbGame = await _igdbApiClient.GetGameByIdAsync(externalId, cancellationToken);

            if (igdbGame is null)
            {
                throw new ApiException(
                    "No fue posible encontrar el videojuego solicitado.",
                    StatusCodes.Status404NotFound);
            }

            var game = ToEntity(igdbGame);
            await AttachGenresAsync(game, igdbGame, cancellationToken);
            await AttachPlatformsAsync(game, igdbGame, cancellationToken);

            _context.Games.Add(game);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                // Dos peticiones a la vez guardaron el mismo juego: el índice único
                // de ExternalId rechaza la segunda. Usamos el que ya quedó guardado.
                _context.ChangeTracker.Clear();
                return await _context.Games.FirstAsync(g => g.ExternalId == externalId, cancellationToken);
            }

            return game;
        }

        /// <summary>Reutiliza GameMapper.GameCard para no repetir cómo se arman las URLs de imagen.</summary>
        private static Game ToEntity(IgdbGameDto igdbGame)
        {
            var card = GameMapper.GameCard(igdbGame);

            return new Game
            {
                ExternalId = card.ExternalId,
                Slug = card.Slug,
                Name = card.Name,
                Description = card.Summary,
                CoverUrl = card.CoverImage,
                BackgroundImageUrl = card.Screenshots.FirstOrDefault(),
                ReleaseDate = card.Released,
                ExternalRating = card.Rating,
                MetacriticScore = card.Metacritic,
                LastSynchronizedAt = DateTime.UtcNow
            };
        }

        private async Task AttachGenresAsync(Game game, IgdbGameDto igdbGame, CancellationToken cancellationToken)
        {
            var igdbGenres = igdbGame.Genres
                .Where(g => g.Id > 0 && !string.IsNullOrWhiteSpace(g.Name))
                .DistinctBy(g => g.Id)
                .ToList();
            var ids = igdbGenres.Select(g => g.Id).ToList();

            var known = await _context.Genres
                .Where(g => ids.Contains(g.ExternalId))
                .ToDictionaryAsync(g => g.ExternalId, cancellationToken);

            foreach (var igdbGenre in igdbGenres)
            {
                var genre = known.GetValueOrDefault(igdbGenre.Id) ?? new Genre
                {
                    ExternalId = igdbGenre.Id,
                    Name = igdbGenre.Name,
                    Slug = igdbGenre.Slug
                };

                game.GameGenres.Add(new GameGenre { Game = game, Genre = genre });
            }
        }

        private async Task AttachPlatformsAsync(Game game, IgdbGameDto igdbGame, CancellationToken cancellationToken)
        {
            var igdbPlatforms = igdbGame.Platforms
                .Where(p => p.Id > 0 && !string.IsNullOrWhiteSpace(p.Name))
                .DistinctBy(p => p.Id)
                .ToList();
            var ids = igdbPlatforms.Select(p => p.Id).ToList();

            var known = await _context.Platforms
                .Where(p => ids.Contains(p.ExternalId))
                .ToDictionaryAsync(p => p.ExternalId, cancellationToken);

            foreach (var igdbPlatform in igdbPlatforms)
            {
                var platform = known.GetValueOrDefault(igdbPlatform.Id) ?? new Platform
                {
                    ExternalId = igdbPlatform.Id,
                    Name = igdbPlatform.Name,
                    Slug = igdbPlatform.Slug
                };

                game.GamePlatforms.Add(new GamePlatform { Game = game, Platform = platform });
            }
        }
    }
}
