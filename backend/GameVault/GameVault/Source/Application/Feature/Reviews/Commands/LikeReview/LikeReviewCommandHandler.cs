using GameVault.Source.Application.Exceptions;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Reviews.Commands.LikeReview
{
    internal sealed class LikeReviewCommandHandler : IRequestHandler<LikeReviewCommand, int>
    {
        private readonly GameVaultContext _context;

        public LikeReviewCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(LikeReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId && r.IsVisible, cancellationToken)
                ?? throw ApiErrors.NotFound("La reseña no existe.");

            if (review.ApplicationUserId == request.UserId)
            {
                throw ApiErrors.BadRequest("No puedes darle me gusta a tu propia reseña.");
            }

            var alreadyLiked = await _context.ReviewLikes.AnyAsync(
                l => l.ReviewId == request.ReviewId && l.ApplicationUserId == request.UserId,
                cancellationToken);

            // Dar me gusta dos veces no es un error: simplemente no cambia nada.
            if (!alreadyLiked)
            {
                _context.ReviewLikes.Add(new ReviewLike
                {
                    ReviewId = request.ReviewId,
                    ApplicationUserId = request.UserId
                });
                await _context.SaveChangesAsync(cancellationToken);
            }

            return await _context.ReviewLikes.CountAsync(l => l.ReviewId == request.ReviewId, cancellationToken);
        }
    }
}
