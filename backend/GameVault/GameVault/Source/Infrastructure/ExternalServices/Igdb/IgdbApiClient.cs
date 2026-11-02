using GameVault.Source.Application.Dtos.Igdb;
using GameVault.Source.Application.Enums;
using GameVault.Source.Application.Interfaces.Igdb;

namespace GameVault.Source.Infrastructure.ExternalServices.Igdb
{
    public sealed class IgdbApiClient : IIgdbApiClient
    {
        private readonly IgdbRequestClient _requestClient;

        public IgdbApiClient(IgdbRequestClient requestClient)
        {
            _requestClient = requestClient;
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
    }
}
