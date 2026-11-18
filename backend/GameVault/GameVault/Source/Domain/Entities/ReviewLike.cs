namespace GameVault.Source.Domain.Entities
{
    /// <summary>
    /// "Me gusta" de un usuario en una reseña.
    /// La clave es (ReviewId, ApplicationUserId): un usuario solo puede dar uno por reseña.
    /// </summary>
    public class ReviewLike
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Navigations:

        // Ref N-1 Review
        public Guid ReviewId { get; set; }

        public Review Review { get; set; } = null!;


        // Ref N-1 ApplicationUser
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
