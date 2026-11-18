using GameVault.Source.Application.Dtos.Reviews;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Reviews.Commands.UpdateReview
{
    internal sealed class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
    {
        private readonly GameVaultContext _context;

        public UpdateReviewCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken)
                ?? throw ApiErrors.NotFound("La reseña no existe.");

            if (review.ApplicationUserId != request.UserId)
            {
                throw ApiErrors.Forbidden("Solo el autor puede editar esta reseña.");
            }

            review.Title = request.Title.Trim();
            review.Content = request.Content.Trim();
            review.Rating = request.Rating;
            review.ContainsSpoilers = request.ContainsSpoilers;
            review.UpdatedAt = DateTime.UtcNow;

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
