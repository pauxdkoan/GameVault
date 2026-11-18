using GameVault.Source.Application.Exceptions;
using GameVault.Source.Infrastructure.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Source.Application.Feature.Comments.Commands.DeleteComment
{
    internal sealed class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
    {
        private readonly GameVaultContext _context;

        public DeleteCommentCommandHandler(GameVaultContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _context.ReviewComments
                .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken)
                ?? throw ApiErrors.NotFound("El comentario no existe.");

            if (comment.ApplicationUserId != request.UserId)
            {
                throw ApiErrors.Forbidden("Solo el autor puede borrar este comentario.");
            }

            _context.ReviewComments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
