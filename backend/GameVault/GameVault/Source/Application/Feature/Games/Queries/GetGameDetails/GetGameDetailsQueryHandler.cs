using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Feature.Games.Queries.GetGames;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Application.Mappings;
using MediatR;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGameDetails
{
    public sealed class GetGameDetailsQueryHandler : IRequestHandler<GetGameDetailsQuery, GameDetailsDto>
    {
        private readonly IIgdbApiClient _igdbApiClient;

        public GetGameDetailsQueryHandler(IIgdbApiClient igdbApiClient)
        {
            _igdbApiClient = igdbApiClient;
        }

        public async Task<GameDetailsDto> Handle(GetGameDetailsQuery request, CancellationToken cancellationToken)
        {
 
            var response = await _igdbApiClient.GetGameByIdAsync(request.Id,cancellationToken);

            return GameMapper.GameDetails(response);
        }
    }
}
