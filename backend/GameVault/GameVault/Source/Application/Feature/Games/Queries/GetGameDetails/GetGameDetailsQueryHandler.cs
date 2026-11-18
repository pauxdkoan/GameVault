using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Application.Interfaces.Igdb;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Games.Queries.GetGameDetails
{
    public sealed class GetGameDetailsQueryHandler : IRequestHandler<GetGameDetailsQuery, GameDetailsDto>
    {
        private readonly IIgdbApiClient _igdbApiClient;
        private readonly GameVaultContext _context;

        public GetGameDetailsQueryHandler(IIgdbApiClient igdbApiClient, GameVaultContext context)
        {
            _igdbApiClient = igdbApiClient;
            _context = context;
        }

        public async Task<GameDetailsDto> Handle(GetGameDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = await _igdbApiClient.GetGameByIdAsync(request.Id, cancellationToken);

            if (response is null)
            {
                throw new ApiException(
                    "No fue posible encontrar el videojuego solicitado.",
                    StatusCodes.Status404NotFound);
            }

            var details = GameMapper.GameDetails(response);

            // Datos propios de GameVault. Si nadie ha usado el juego, no hay nada guardado.
            var reviews = _context.Reviews
                .AsNoTracking()
                .Where(r => r.Game.ExternalId == details.ExternalId && r.IsVisible);

            details.ReviewCount = await reviews.CountAsync(cancellationToken);

            var average = await reviews.AverageAsync(r => (double?)r.Rating, cancellationToken);
            details.CommunityRating = average is null ? null : Math.Round((decimal)average.Value, 1);

            if (request.CurrentUserId is not null)
            {
                var entry = await _context.UserGames
                    .AsNoTracking()
                    .IncludeCardData()
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(
                        ug => ug.ApplicationUserId == request.CurrentUserId && ug.Game.ExternalId == details.ExternalId,
                        cancellationToken);

                details.MyLibraryEntry = entry is null ? null : LibraryMapper.ToDto(entry);
            }

            return details;
        }
    }
}
