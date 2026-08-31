using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Application.Mappings;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.SearchGame
{
    public sealed class SearchGamesQueryHandler : IRequestHandler<SearchGamesQuery, GamesPageDto>
    {
        private readonly IIgdbApiClient _igdbApiClient;

        public SearchGamesQueryHandler(IIgdbApiClient igdbApiClient)
        {
            _igdbApiClient = igdbApiClient;
        }

        public async Task<GamesPageDto> Handle(SearchGamesQuery request, CancellationToken cancellationToken)
        {
            var pageSize = Math.Clamp(request.PageSize, 1, 40);
            var page = Math.Max(request.Page, 1);

            var response = await _igdbApiClient.SearchGamesAsync(
                request.Query,
                page,
                pageSize,
                cancellationToken);

            return new GamesPageDto
            {
                Count = response.Count,
                Page = page,
                PageSize = pageSize,
                Games = response.Results
                    .Select(GameMapper.GameSummary)
                    .ToList()
            };
        }
    }
}
