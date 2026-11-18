using MediatR;

namespace GameVault.Source.Application.Feature.Lists.Commands.DeleteList
{
    /// <summary>Borra la lista y sus items (cascada). Los juegos no se tocan.</summary>
    public sealed record DeleteListCommand(Guid UserId, Guid ListId) : IRequest<Unit>;
}
