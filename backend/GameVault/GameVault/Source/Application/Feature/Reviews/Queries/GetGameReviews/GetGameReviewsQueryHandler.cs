using GameVault.Source.Application.Dtos.Reviews;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Reviews.Queries.GetGameReviews
{
    internal sealed class GetGameReviewsQueryHandler : IRequestHandler<GetGameReviewsQuery, GameReviewsDto>
    {
        private readonly GameVaultContext _context;

        public GetGameReviewsQueryHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<GameReviewsDto> Handle(GetGameReviewsQuery request, CancellationToken cancellationToken)
        {
            // Si nadie ha usado el juego todavía, no está en la base de datos: no hay reseñas.
            var gameReviews = _context.Reviews
                .AsNoTracking()
                .Where(r => r.Game.ExternalId == request.ExternalId && r.IsVisible);

            var average = await gameReviews.AverageAsync(r => (double?)r.Rating, cancellationToken);

            var page = await gameReviews
                .ToRows(request.CurrentUserId)
                .OrderByDescending(row => row.Review.CreatedAt)
                .ToPagedResultAsync(
                    request.Page,
                    request.PageSize,
                    row => ReviewQueries.ToDto(row, request.CurrentUserId),
                    cancellationToken);

            ReviewDto? myReview = null;

            if (request.CurrentUserId is not null)
            {
                var myRow = await gameReviews
                    .Where(r => r.ApplicationUserId == request.CurrentUserId)
                    .ToRows(request.CurrentUserId)
                    .FirstOrDefaultAsync(cancellationToken);

                myReview = myRow is null ? null : ReviewQueries.ToDto(myRow, request.CurrentUserId);
            }

            return new GameReviewsDto
            {
                Reviews = page,
                AverageRating = average is null ? null : Math.Round((decimal)average.Value, 1),
                ReviewCount = page.Count,
                MyReview = myReview
            };
        }
    }
}
