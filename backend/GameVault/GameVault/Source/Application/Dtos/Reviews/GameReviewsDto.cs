using GameVault.Source.Application.Dtos.Common;

namespace GameVault.Source.Application.Dtos.Reviews
{
    /// <summary>Reseñas de un juego, la media de la comunidad y la reseña del usuario (si tiene).</summary>
    public sealed class GameReviewsDto
    {
        public PagedResultDto<ReviewDto> Reviews { get; set; } = new();

        /// <summary>Media de las notas (1 a 10) con un decimal. null si no hay reseñas.</summary>
        public decimal? AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public ReviewDto? MyReview { get; set; }
    }
}
