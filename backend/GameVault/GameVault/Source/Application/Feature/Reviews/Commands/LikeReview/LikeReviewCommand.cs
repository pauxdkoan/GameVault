using MediatR;

namespace GameVault.Source.Application.Feature.Reviews.Commands.LikeReview
{
    /// <summary>Da me gusta. Devuelve el total de me gusta de la reseña.</summary>
    public sealed record LikeReviewCommand(Guid UserId, Guid ReviewId) : IRequest<int>;
}
