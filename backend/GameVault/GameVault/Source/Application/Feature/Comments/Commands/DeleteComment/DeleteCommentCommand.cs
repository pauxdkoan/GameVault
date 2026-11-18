using MediatR;

namespace GameVault.Source.Application.Feature.Comments.Commands.DeleteComment
{
    public sealed record DeleteCommentCommand(Guid UserId, Guid CommentId) : IRequest<Unit>;
}
