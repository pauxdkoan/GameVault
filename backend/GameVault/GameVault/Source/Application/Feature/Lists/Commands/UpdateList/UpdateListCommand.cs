using GameVault.Source.Application.Dtos.Lists;
using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Commands.UpdateList
{
    public sealed record UpdateListCommand(
        Guid UserId,
        Guid ListId,
        string Name,
        string? Description,
        bool IsPublic) : IRequest<GameListSummaryDto>;
}
