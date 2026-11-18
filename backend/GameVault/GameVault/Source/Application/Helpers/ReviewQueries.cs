using GameVault.Source.Application.Dtos.Reviews;
using GameVault.Source.Application.Mappings;
using GameVault.Source.Domain.Entities;

namespace GameVault.Source.Application.Helpers
{
    /// <summary>
    /// Consulta común de reseñas: trae la reseña, su autor, su juego y los contadores
    /// en una sola consulta SQL (sin cargar todos los me gusta y comentarios).
    /// La usan el detalle del juego, el perfil y la actividad.
    /// </summary>
    internal static class ReviewQueries
    {
        internal sealed class ReviewRow
        {
            public Review Review { get; init; } = null!;
            public ApplicationUser Author { get; init; } = null!;
            public Game Game { get; init; } = null!;
            public int LikeCount { get; init; }
            public int CommentCount { get; init; }
            public bool LikedByMe { get; init; }
        }

        public static IQueryable<ReviewRow> ToRows(this IQueryable<Review> query, Guid? currentUserId)
        {
            return query
                .Where(r => r.IsVisible)
                .Select(r => new ReviewRow
                {
                    Review = r,
                    Author = r.ApplicationUser,
                    Game = r.Game,
                    LikeCount = r.Likes.Count,
                    CommentCount = r.Comments.Count,
                    LikedByMe = currentUserId != null && r.Likes.Any(l => l.ApplicationUserId == currentUserId)
                });
        }

        public static ReviewDto ToDto(ReviewRow row, Guid? currentUserId)
        {
            return new ReviewDto
            {
                Id = row.Review.Id,
                GameExternalId = row.Game.ExternalId,
                GameName = row.Game.Name,
                GameCoverImage = row.Game.CoverUrl,
                Author = GameEntityMapper.ToUserSummary(row.Author),
                Title = row.Review.Title,
                Content = row.Review.Content,
                Rating = row.Review.Rating,
                ContainsSpoilers = row.Review.ContainsSpoilers,
                LikeCount = row.LikeCount,
                CommentCount = row.CommentCount,
                LikedByMe = row.LikedByMe,
                IsMine = currentUserId == row.Review.ApplicationUserId,
                CreatedAt = row.Review.CreatedAt,
                UpdatedAt = row.Review.UpdatedAt
            };
        }
    }
}
