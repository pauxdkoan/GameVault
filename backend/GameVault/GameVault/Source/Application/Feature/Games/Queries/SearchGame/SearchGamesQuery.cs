using GameVault.Source.Application.Dtos.Game;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.SearchGame
{
    public sealed record SearchGamesQuery(
        string Query,
        int Page = 1,
        int PageSize = 20) : IRequest<GamesPageDto>;
}
