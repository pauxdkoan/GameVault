using GameVault.Source.Application.Dtos.Game;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGameDetails
{
    public sealed record GetGameDetailsQuery(
        long Id
     ) : IRequest<GameDetailsDto>;
}
