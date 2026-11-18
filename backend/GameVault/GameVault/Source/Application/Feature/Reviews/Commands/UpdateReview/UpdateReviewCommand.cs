using GameVault.Source.Application.Dtos.Reviews;
using MediatR;

namespace GameVault.Source.Application.Feature.Reviews.Commands.UpdateReview
{
    public sealed record UpdateReviewCommand(
        Guid UserId,
        Guid ReviewId,
        string Title,
        string Content,
        int Rating,
        bool ContainsSpoilers) : IRequest<ReviewDto>;
}
