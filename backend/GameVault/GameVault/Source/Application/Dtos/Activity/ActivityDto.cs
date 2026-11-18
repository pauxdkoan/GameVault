using GameVault.Source.Application.Dtos.User;

namespace GameVault.Source.Application.Dtos.Activity
{
    public enum ActivityType
    {
        /// <summary>Escribió una reseña.</summary>
        Review,
        /// <summary>Marcó un juego como completado.</summary>
        Completed,
        /// <summary>Creó una lista pública.</summary>
        ListCreated
    }

    /// <summary>
    /// Un evento de la comunidad para el home ("Ana completó Hades").
    /// Según Type vienen rellenos unos campos u otros.
    /// </summary>
    public sealed class ActivityDto
    {
        public ActivityType Type { get; set; }

        public UserSummaryDto User { get; set; } = new();

        public DateTime OccurredAt { get; set; }

        // Review y Completed
        public int? GameExternalId { get; set; }
        public string? GameName { get; set; }
        public string? GameCoverImage { get; set; }

        // Review
        public Guid? ReviewId { get; set; }
        public int? Rating { get; set; }
        /// <summary>Primeros 160 caracteres de la reseña (vacío si tiene spoilers).</summary>
        public string? Excerpt { get; set; }
        public int? LikeCount { get; set; }
        public int? CommentCount { get; set; }

        // ListCreated
        public Guid? ListId { get; set; }
        public string? ListName { get; set; }
        public int? ListGameCount { get; set; }
    }
}
