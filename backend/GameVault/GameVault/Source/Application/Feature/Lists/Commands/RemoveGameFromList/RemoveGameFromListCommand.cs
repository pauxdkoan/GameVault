using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Commands.RemoveGameFromList
{
    /// <summary>Quita un juego de la lista y recoloca los demás para que no queden huecos.</summary>
    public sealed record RemoveGameFromListCommand(Guid UserId, Guid ListId, int ExternalId) : IRequest<Unit>;
}
