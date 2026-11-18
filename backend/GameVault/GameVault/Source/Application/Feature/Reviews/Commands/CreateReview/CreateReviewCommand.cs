using GameVault.Source.Application.Dtos.Reviews;
using MediatR;

namespace GameVault.Source.Application.Feature.Reviews.Commands.CreateReview
{
    public sealed record CreateReviewCommand(
        Guid UserId,
        int ExternalId,
        string Title,
        string Content,
        int Rating,
        bool ContainsSpoilers) : IRequest<ReviewDto>;

    /// <summary>Cuerpo del POST /api/games/{externalId}/reviews y del PUT /api/reviews/{id}.</summary>
    public sealed record ReviewRequest(
        string? Title,
        string Content,
        int Rating,
        bool ContainsSpoilers = false);
}
