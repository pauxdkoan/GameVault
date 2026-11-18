using GameVault.Source.Application.Dtos.Game;
using GameVault.Source.Domain.Enums;

namespace GameVault.Source.Application.Dtos.Library
{
    /// <summary>Un juego en la biblioteca del usuario, con su estado y progreso.</summary>
    public sealed class LibraryEntryDto
    {
        public GameCardDto Game { get; set; } = new();

        public GameStatus Status { get; set; }

        public int ProgressPercentage { get; set; }

        public decimal? HoursPlayed { get; set; }

        /// <summary>Nota personal de 1 a 10.</summary>
        public int? PersonalRating { get; set; }

        public DateOnly? StartedAt { get; set; }

        public DateOnly? CompletedAt { get; set; }

        public string? PrivateNotes { get; set; }

        public DateTime AddedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
