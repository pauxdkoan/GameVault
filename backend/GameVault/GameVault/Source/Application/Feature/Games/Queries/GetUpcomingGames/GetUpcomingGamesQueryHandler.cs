using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Feature.Games.Queries.GetGames;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Application.Mappings;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetUpcomingGames
{
    public sealed class GetUpcomingGamesQueryHandler : IRequestHandler<GetUpcomingGamesQuery, GamesPageDto>
    {
        private readonly IIgdbApiClient _igdbApiClient;

        public GetUpcomingGamesQueryHandler(IIgdbApiClient igdbApiClient)
        {
            _igdbApiClient = igdbApiClient;
        }

        public async Task<GamesPageDto> Handle(GetUpcomingGamesQuery request, CancellationToken cancellationToken)
        {
            var pageSize = Math.Clamp(request.PageSize, 1, 40);
            var page = Math.Max(request.Page, 1);

            var response = await _igdbApiClient.GetUpcomingGamesAsync(page, pageSize, cancellationToken);

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
