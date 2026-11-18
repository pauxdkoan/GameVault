using GameVault.Source.Application.Dtos.Common;
using GameVault.Source.Application.Dtos.Lists;
using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Queries.GetPublicLists
{
    /// <summary>Listas públicas de todos los usuarios, las más recientes primero.</summary>
    public sealed record GetPublicListsQuery(int Page = 1, int PageSize = 12)
        : IRequest<PagedResultDto<GameListSummaryDto>>;
}
