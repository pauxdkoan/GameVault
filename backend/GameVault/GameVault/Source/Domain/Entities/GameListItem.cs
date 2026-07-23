using GameVault.Source.Domain.Common;

namespace GameVault.Source.Domain.Entities
{
    public class GameListItem : BaseEntity
    {
        public int Position { get; set; }

        public string? Note { get; set; }


        // Navigations:

        // Ref N-1 GameList
        public Guid GameListId { get; set; }

        public GameList GameList { get; set; } = null!;


        // Ref N-1 Game
        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;
    }
}
