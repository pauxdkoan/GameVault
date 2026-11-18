using GameVault.Source.Application.Dtos.Common;
using GameVault.Source.Application.Dtos.Reviews;
using MediatR;

namespace GameVault.Source.Application.Feature.Comments.Queries.GetReviewComments
{
    /// <summary>Comentarios de una reseña, del más antiguo al más nuevo (como una conversación).</summary>
    public sealed record GetReviewCommentsQuery(
        Guid ReviewId,
        Guid? CurrentUserId,
        int Page = 1,
        int PageSize = 20) : IRequest<PagedResultDto<CommentDto>>;
}
