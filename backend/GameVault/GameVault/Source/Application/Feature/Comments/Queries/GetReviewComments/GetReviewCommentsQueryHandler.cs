using GameVault.Source.Application.Dtos.Common;
using GameVault.Source.Application.Dtos.Reviews;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Helpers;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Comments.Queries.GetReviewComments
{
    internal sealed class GetReviewCommentsQueryHandler
        : IRequestHandler<GetReviewCommentsQuery, PagedResultDto<CommentDto>>
    {
        private readonly GameVaultContext _context;

        public GetReviewCommentsQueryHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<CommentDto>> Handle(GetReviewCommentsQuery request, CancellationToken cancellationToken)
        {
            var reviewExists = await _context.Reviews.AnyAsync(
                r => r.Id == request.ReviewId && r.IsVisible, cancellationToken);

            if (!reviewExists)
            {
                throw ApiErrors.NotFound("La reseña no existe.");
            }

            return await _context.ReviewComments
                .AsNoTracking()
                .Include(c => c.ApplicationUser)
                .Where(c => c.ReviewId == request.ReviewId)
                .OrderBy(c => c.CreatedAt)
                .ToPagedResultAsync(
                    request.Page,
                    request.PageSize,
                    c => CommentMapper.ToDto(c, request.CurrentUserId),
                    cancellationToken);
        }
    }
}
