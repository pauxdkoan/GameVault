using MediatR;

namespace GameVault.Source.Application.Feature.Reviews.Commands.UnlikeReview
{
    /// <summary>Quita el me gusta. Devuelve el total de me gusta de la reseña.</summary>
    public sealed record UnlikeReviewCommand(Guid UserId, Guid ReviewId) : IRequest<int>;
}
