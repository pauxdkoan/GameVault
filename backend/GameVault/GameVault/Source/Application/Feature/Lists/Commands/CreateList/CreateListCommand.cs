using GameVault.Source.Application.Dtos.Lists;
using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Commands.CreateList
{
    public sealed record CreateListCommand(
        Guid UserId,
        string Name,
        string? Description,
        bool IsPublic) : IRequest<GameListSummaryDto>;

    /// <summary>Cuerpo del POST /api/lists y del PUT /api/lists/{id}.</summary>
    public sealed record ListRequest(string Name, string? Description, bool IsPublic = true);
}
