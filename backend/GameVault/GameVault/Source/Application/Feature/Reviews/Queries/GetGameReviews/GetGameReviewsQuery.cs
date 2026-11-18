using GameVault.Source.Application.Dtos.Reviews;
using MediatR;

namespace GameVault.Source.Application.Feature.Reviews.Queries.GetGameReviews
{
    /// <summary>Reseñas de un juego. CurrentUserId es null si no hay sesión.</summary>
    public sealed record GetGameReviewsQuery(
        int ExternalId,
        Guid? CurrentUserId,
        int Page = 1,
        int PageSize = 10) : IRequest<GameReviewsDto>;
}
