using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Enums;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGames
{
    public sealed record GetGamesQuery(
        string? Search = null,
        IReadOnlyCollection<int>? GenreIds = null,
        IReadOnlyCollection<int>? PlatformIds = null,
        GameSort Sort = GameSort.Popular,
        int Page = 1,
        int PageSize = 20) : IRequest<GamesPageDto>;
}
