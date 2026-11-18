using GameVault.Source.Application.Exceptions;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Reviews.Commands.DeleteReview
{
    internal sealed class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Unit>
    {
        private readonly GameVaultContext _context;

        public DeleteReviewCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken)
                ?? throw ApiErrors.NotFound("La reseña no existe.");

            if (review.ApplicationUserId != request.UserId)
            {
                throw ApiErrors.Forbidden("Solo el autor puede borrar esta reseña.");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
