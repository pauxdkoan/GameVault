using MediatR;

namespace GameVault.Source.Application.Feature.Reviews.Commands.DeleteReview
{
    /// <summary>Borra la reseña con sus comentarios y me gusta (cascada en la base de datos).</summary>
    public sealed record DeleteReviewCommand(Guid UserId, Guid ReviewId) : IRequest<Unit>;
}
