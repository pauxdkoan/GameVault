using GameVault.Source.Domain.Common;

namespace GameVault.Source.Domain.Entities
{
    public class GameList : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? CoverImageUrl { get; set; }

        public bool IsPublic { get; set; }


        // Navigations:

        // Ref N-1 ApplicationUser
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;


        // Ref 1-N GameListItem
        public ICollection<GameListItem> Items { get; set; } = [];
    }
}
