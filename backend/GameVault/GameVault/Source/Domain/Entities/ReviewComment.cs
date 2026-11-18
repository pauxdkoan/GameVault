using GameVault.Source.Domain.Common;

namespace GameVault.Source.Domain.Entities
{
    /// <summary>Comentario que un usuario deja en la reseña de otro (o en la suya).</summary>
    public class ReviewComment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;


        // Navigations:

        // Ref N-1 ApplicationUser (autor del comentario)
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;


        // Ref N-1 Review
        public Guid ReviewId { get; set; }

        public Review Review { get; set; } = null!;
    }
}
