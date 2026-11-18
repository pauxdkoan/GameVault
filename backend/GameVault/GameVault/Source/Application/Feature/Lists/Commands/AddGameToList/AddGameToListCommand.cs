using GameVault.Source.Application.Dtos.Lists;
using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Commands.AddGameToList
{
    /// <summary>Añade un juego al final de la lista.</summary>
    public sealed record AddGameToListCommand(
        Guid UserId,
        Guid ListId,
        int ExternalId,
        string? Note) : IRequest<GameListItemDto>;

    /// <summary>Cuerpo (opcional) del POST /api/lists/{id}/games/{externalId}.</summary>
    public sealed record AddGameToListRequest(string? Note);
}
