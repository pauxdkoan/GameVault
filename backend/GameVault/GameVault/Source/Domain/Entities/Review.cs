using GameVault.Source.Domain.Common;

namespace GameVault.Source.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int Rating { get; set; }

        public bool ContainsSpoilers { get; set; }

        public bool IsVisible { get; set; } = true;


        // Navigations:

        // Ref N-1 ApplicationUser
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;


        // Ref N-1 Game
        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;
    }
}
