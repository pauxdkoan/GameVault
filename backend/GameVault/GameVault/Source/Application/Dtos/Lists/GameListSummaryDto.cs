using GameVault.Source.Application.Dtos.User;

namespace GameVault.Source.Application.Dtos.Lists
{
    /// <summary>Lista para mostrar en una cuadrícula: nombre, dueño, cuántos juegos y hasta 4 portadas.</summary>
    public sealed class GameListSummaryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }

        public int GameCount { get; set; }

        public List<string> CoverImages { get; set; } = [];

        public UserSummaryDto Owner { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
