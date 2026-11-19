using GameVault.Source.Application.Dtos.Igdb;
using GameVault.Source.Application.Enums;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Interfaces.Igdb;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;

namespace GameVault.Source.Infrastructure.ExternalServices.Igdb
{
    public sealed class IgdbApiClient : IIgdbApiClient
    {
        /// <summary>Tipo de popularidad de IGDB: 1 = visitas a la ficha.</summary>
        private const int PopularityTypeVisits = 1;

        /// <summary>Cuántos juegos populares se piden (500 es el máximo de IGDB por consulta).</summary>
        private const int PopularPoolSize = 500;

        private const string PopularNowCacheKey = "igdb:popular-now";

        /// <summary>La popularidad cambia poco en unas horas: así se ahorran llamadas a IGDB.</summary>
        private static readonly TimeSpan PopularNowCacheDuration = TimeSpan.FromHours(6);

        private readonly IgdbRequestClient _requestClient;
        private readonly IMemoryCache _cache;

        public IgdbApiClient(IgdbRequestClient requestClient, IMemoryCache cache)
        {
            _requestClient = requestClient;
            _cache = cache;
        }

        public async Task<IgdbGamesResponse> GetGamesAsync(
            string? search,
            IReadOnlyCollection<int>? genreIds,
            IReadOnlyCollection<int>? platformIds,
            GameSort sort,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            // Catálogo por defecto (orden "Popular", sin búsqueda ni filtros):
            // se usa la popularidad actual de IGDB en vez del total de valoraciones,
            // que siempre ponía arriba juegos antiguos (The Witcher 3, Skyrim...).
            if (IsPopularNowRequest(search, genreIds, platformIds, sort))
            {
                var popularNow = await TryGetPopularNowPageAsync(page, pageSize, cancellationToken);
                if (popularNow is not null)
                {
                    return popularNow;
                }
            }

            var query = IgdbGameQueryBuilder.BuildGamesPage(
                search,
                genreIds,
                platformIds,
                sort,
                page,
                pageSize);

            return await GetGamesPageAsync(query, cancellationToken);
        }

        public async Task<IgdbGamesResponse> SearchGamesAsync(
            string search,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = IgdbGameQueryBuilder.BuildSearchGamesPage(search, page, pageSize);

            return await GetGamesPageAsync(query, cancellationToken);
        }

        public async Task<IgdbGamesResponse> GetTrendingGamesAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = IgdbGameQueryBuilder.BuildTrendingGamesPage(page, pageSize);

            return await GetGamesPageAsync(query, cancellationToken);
        }

        public async Task<IgdbGamesResponse> GetUpcomingGamesAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = IgdbGameQueryBuilder.BuildUpcomingGamesPage(page, pageSize);

            return await GetGamesPageAsync(query, cancellationToken);
        }


        public async Task<IgdbGameDto?> GetGameByIdAsync(
            long id,
            CancellationToken cancellationToken)
        {
            var query = IgdbGameQueryBuilder.BuildGameDetailPageById(id);

            var games = await _requestClient.PostAsync<List<IgdbGameDto>>(
                "games",
                query,
                cancellationToken);

            return games?.FirstOrDefault();
        }

        private static bool IsPopularNowRequest(
            string? search,
            IReadOnlyCollection<int>? genreIds,
            IReadOnlyCollection<int>? platformIds,
            GameSort sort)
        {
            return sort == GameSort.Popular
                && string.IsNullOrWhiteSpace(search)
                && (genreIds is null || genreIds.Count == 0)
                && (platformIds is null || platformIds.Count == 0);
        }

        /// <summary>
        /// Página del ranking "Popular ahora". Devuelve null si IGDB falla o no
        /// trae datos, y entonces se usa el orden de siempre (no se rompe el catálogo).
        /// </summary>
        private async Task<IgdbGamesResponse?> TryGetPopularNowPageAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            try
            {
                var rankedIds = await GetPopularNowIdsAsync(cancellationToken);
                if (rankedIds.Count == 0)
                {
                    return null;
                }

                var pageIds = rankedIds
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (pageIds.Count == 0)
                {
                    return new IgdbGamesResponse { Count = rankedIds.Count, Results = [] };
                }

                var games = await _requestClient.PostAsync<List<IgdbGameDto>>(
                    "games",
                    IgdbGameQueryBuilder.BuildGamesByIdsQuery(pageIds),
                    cancellationToken)
                    ?? [];

                // IGDB devuelve los juegos en cualquier orden: se reordenan según el ranking.
                var position = pageIds
                    .Select((id, index) => (id, index))
                    .ToDictionary(item => item.id, item => item.index);

                return new IgdbGamesResponse
                {
                    Count = rankedIds.Count,
                    Results = games
                        .OrderBy(game => position.GetValueOrDefault(game.Id, int.MaxValue))
                        .ToList()
                };
            }
            catch (ApiException)
            {
                return null;
            }
        }

        /// <summary>
        /// Ids de los juegos más visitados en IGDB, de más a menos, ya filtrados
        /// (juego principal, con portada, sin contenido adulto). Se guardan 6 horas en memoria.
        /// </summary>
        private async Task<IReadOnlyList<int>> GetPopularNowIdsAsync(CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(PopularNowCacheKey, out IReadOnlyList<int>? cachedIds) && cachedIds is not null)
            {
                return cachedIds;
            }

            var primitives = await _requestClient.PostAsync<List<IgdbPopularityPrimitive>>(
                "popularity_primitives",
                IgdbGameQueryBuilder.BuildPopularityQuery(PopularityTypeVisits, PopularPoolSize),
                cancellationToken)
                ?? [];

            var candidateIds = primitives
                .Select(primitive => primitive.GameId)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (candidateIds.Count == 0)
            {
                return [];
            }

            var validGames = await _requestClient.PostAsync<List<IgdbGameDto>>(
                "games",
                IgdbGameQueryBuilder.BuildValidGameIdsQuery(candidateIds),
                cancellationToken)
                ?? [];

            var validIds = validGames.Select(game => game.Id).ToHashSet();
            IReadOnlyList<int> rankedIds = candidateIds.Where(validIds.Contains).ToList();

            if (rankedIds.Count > 0)
            {
                _cache.Set(PopularNowCacheKey, rankedIds, PopularNowCacheDuration);
            }

            return rankedIds;
        }

        private async Task<IgdbGamesResponse> GetGamesPageAsync(
            IgdbPageQuery query,
            CancellationToken cancellationToken)
        {
            var games = await _requestClient.PostAsync<List<IgdbGameDto>>(
                "games",
                query.ResultsQuery,
                cancellationToken)
                ?? [];

            var count = await _requestClient.CountAsync(
                "games/count",
                query.CountQuery,
                cancellationToken);

            return new IgdbGamesResponse
            {
                Count = count,
                Results = games
            };
        }

        /// <summary>Una fila de popularity_primitives de IGDB.</summary>
        private sealed class IgdbPopularityPrimitive
        {
            [JsonPropertyName("game_id")]
            public int GameId { get; set; }

            [JsonPropertyName("value")]
            public double Value { get; set; }
        }
    }
}
