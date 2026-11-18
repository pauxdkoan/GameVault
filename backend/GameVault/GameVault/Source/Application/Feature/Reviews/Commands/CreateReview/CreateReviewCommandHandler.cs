using GameVault.Source.Application.Dtos.Reviews;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Application.Interfaces.Games;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Reviews.Commands.CreateReview
{
    internal sealed class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
    {
        private readonly GameVaultContext _context;
        private readonly IGameSyncService _gameSyncService;

        public CreateReviewCommandHandler(GameVaultContext context, IGameSyncService gameSyncService)
        {
            _context = context;
            _gameSyncService = gameSyncService;
        }

        public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var game = await _gameSyncService.EnsureGameAsync(request.ExternalId, cancellationToken);

            var alreadyReviewed = await _context.Reviews.AnyAsync(
                r => r.ApplicationUserId == request.UserId && r.GameId == game.Id,
                cancellationToken);

            if (alreadyReviewed)
            {
                throw ApiErrors.Conflict("Ya escribiste una reseña de este juego. Puedes editarla.");
            }

            var review = new Review
            {
                ApplicationUserId = request.UserId,
                GameId = game.Id,
                Title = request.Title.Trim(),
                Content = request.Content.Trim(),
                Rating = request.Rating,
                ContainsSpoilers = request.ContainsSpoilers
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);

            var row = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.Id == review.Id)
                .ToRows(request.UserId)
                .FirstAsync(cancellationToken);

            return ReviewQueries.ToDto(row, request.UserId);
        }
    }
}
