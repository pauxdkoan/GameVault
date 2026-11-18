using GameVault.Source.Application.Dtos.Game;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGameDetails
{
    /// <summary>Detalle de un juego. CurrentUserId es null si no hay sesión.</summary>
    public sealed record GetGameDetailsQuery(
        long Id,
        Guid? CurrentUserId = null
     ) : IRequest<GameDetailsDto>;
}
