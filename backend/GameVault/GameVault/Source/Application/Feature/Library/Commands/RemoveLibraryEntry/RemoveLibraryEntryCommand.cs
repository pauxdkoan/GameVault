using MediatR;

namespace GameVault.Source.Application.Feature.Library.Commands.RemoveLibraryEntry
{
    /// <summary>Quita un juego de la biblioteca. Si no estaba, no hace nada.</summary>
    public sealed record RemoveLibraryEntryCommand(Guid UserId, int ExternalId) : IRequest<Unit>;
}
