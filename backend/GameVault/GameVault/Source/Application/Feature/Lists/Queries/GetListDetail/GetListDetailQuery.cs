using GameVault.Source.Application.Dtos.Lists;
using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Queries.GetListDetail
{
    /// <summary>Una lista con sus juegos. Las privadas solo las ve su dueño.</summary>
    public sealed record GetListDetailQuery(Guid ListId, Guid? CurrentUserId) : IRequest<GameListDetailDto>;
}
