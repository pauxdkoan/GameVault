using GameVault.Source.Application.Dtos.User;

namespace GameVault.Source.Application.Dtos.Reviews
{
    public sealed class ReviewDto
    {
        public Guid Id { get; set; }

        // Datos mínimos del juego, para mostrar la reseña fuera del detalle (perfil, actividad).
        public int GameExternalId { get; set; }

        public string GameName { get; set; } = string.Empty;

        public string? GameCoverImage { get; set; }

        public UserSummaryDto Author { get; set; } = new();

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        /// <summary>Nota de 1 a 10.</summary>
        public int Rating { get; set; }

        public bool ContainsSpoilers { get; set; }

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }

        /// <summary>true si el usuario que pide los datos le dio me gusta.</summary>
        public bool LikedByMe { get; set; }

        /// <summary>true si el usuario que pide los datos es el autor (puede editar y borrar).</summary>
        public bool IsMine { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
