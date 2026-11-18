using GameVault.Source.Application.Dtos.Lists;
using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Queries.GetMyLists
{
    /// <summary>Todas las listas del usuario (públicas y privadas).</summary>
    public sealed record GetMyListsQuery(Guid UserId) : IRequest<List<GameListSummaryDto>>;
}
