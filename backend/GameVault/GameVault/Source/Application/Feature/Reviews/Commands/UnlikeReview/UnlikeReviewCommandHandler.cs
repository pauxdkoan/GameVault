using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Reviews.Commands.UnlikeReview
{
    internal sealed class UnlikeReviewCommandHandler : IRequestHandler<UnlikeReviewCommand, int>
    {
        private readonly GameVaultContext _context;

        public UnlikeReviewCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(UnlikeReviewCommand request, CancellationToken cancellationToken)
        {
            var like = await _context.ReviewLikes.FirstOrDefaultAsync(
                l => l.ReviewId == request.ReviewId && l.ApplicationUserId == request.UserId,
                cancellationToken);

            if (like is not null)
            {
                _context.ReviewLikes.Remove(like);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return await _context.ReviewLikes.CountAsync(l => l.ReviewId == request.ReviewId, cancellationToken);
        }
    }
}
