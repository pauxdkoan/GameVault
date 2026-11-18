using GameVault.Source.Application.Dtos.Reviews;
using MediatR;

namespace GameVault.Source.Application.Feature.Comments.Commands.AddComment
{
    public sealed record AddCommentCommand(Guid UserId, Guid ReviewId, string Content) : IRequest<CommentDto>;

    /// <summary>Cuerpo del POST /api/reviews/{id}/comments.</summary>
    public sealed record AddCommentRequest(string Content);
}
