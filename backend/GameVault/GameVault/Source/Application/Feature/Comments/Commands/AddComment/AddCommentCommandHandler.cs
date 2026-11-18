using GameVault.Source.Application.Dtos.Reviews;
using GameVault.Source.Application.Exceptions;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Domain.Entities;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Comments.Commands.AddComment
{
    internal sealed class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto>
    {
        private readonly GameVaultContext _context;

        public AddCommentCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var reviewExists = await _context.Reviews.AnyAsync(
                r => r.Id == request.ReviewId && r.IsVisible, cancellationToken);

            if (!reviewExists)
            {
                throw ApiErrors.NotFound("La reseña no existe.");
            }

            var comment = new ReviewComment
            {
                ReviewId = request.ReviewId,
                ApplicationUserId = request.UserId,
                Content = request.Content.Trim()
            };

            _context.ReviewComments.Add(comment);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(comment).Reference(c => c.ApplicationUser).LoadAsync(cancellationToken);

            return CommentMapper.ToDto(comment, request.UserId);
        }
    }
}
