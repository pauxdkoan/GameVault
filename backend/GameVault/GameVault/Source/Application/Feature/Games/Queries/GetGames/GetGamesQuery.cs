using GameVault.Source.Application.Dtos.Game;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGames
{
    public sealed record GetGamesQuery(
        int Page = 1,
        int PageSize = 20) : IRequest<GamesPageDto>;
}
