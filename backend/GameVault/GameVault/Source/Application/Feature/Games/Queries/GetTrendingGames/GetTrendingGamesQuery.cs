using GameVault.Source.Application.Dtos.Game;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetTrendingGames
{
    public sealed record GetTrendingGamesQuery(
        int Page = 1,
        int PageSize = 20) : IRequest<GamesPageDto>;
}
