using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Feature.Games.Queries.GetGames;
using GameVault.Source.Application.Interfaces.Rawg;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.SearchGame
{
    public sealed class GetGamesQueryHandler : IRequestHandler<SearchGamesQuery, GamesPageDto>
    {
        private readonly IRawgApiClient _rawgApiClient;

        public GetGamesQueryHandler(IRawgApiClient rawgApiClient)
        {
            _rawgApiClient = rawgApiClient;
        }

        public async Task<GamesPageDto> Handle(SearchGamesQuery request, CancellationToken cancellationToken)
        {
            var pageSize = Math.Clamp(request.PageSize, 1, 40);
            var page = Math.Max(request.Page, 1);

            var response = await _rawgApiClient.SearchGamesAsync(request.Query,page, pageSize, cancellationToken);

            return new GamesPageDto
            {
                Count = response.Count,
                Page = page,
                PageSize = pageSize,
                Games = response.Results
                .Select(game => new GameSummaryDto
                {
                    ExternalId = game.Id,
                    Slug = game.Slug,
                    Name = game.Name,
                    Released = game.Released,
                    BackgroundImage = game.BackgroundImage,
                    Rating = game.Rating,
                    Metacritic = game.Metacritic,

                    Genres = game.Genres
                        .Select(genre => genre.Name)
                        .ToList()
                })
                .ToList()
            };

        }
    }
}
