using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Application.Dtos.User;

namespace GameVault.Source.Application.Dtos.Lists
{
    public sealed class GameListDetailDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }

        public UserSummaryDto Owner { get; set; } = new();

        /// <summary>true si quien pide la lista es el dueño (puede editarla).</summary>
        public bool IsMine { get; set; }

        public List<GameListItemDto> Items { get; set; } = [];

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public sealed class GameListItemDto
    {
        public int Position { get; set; }

        public string? Note { get; set; }

        public GameCardDto Game { get; set; } = new();
    }
}
