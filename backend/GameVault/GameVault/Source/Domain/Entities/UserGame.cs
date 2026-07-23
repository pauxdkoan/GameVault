using GameVault.Source.Domain.Common;
using GameVault.Source.Domain.Enums;

namespace GameVault.Source.Domain.Entities
{
    public class UserGame : BaseEntity
    {
        public GameStatus Status { get; set; }

        public int ProgressPercentage { get; set; }

        public decimal? HoursPlayed { get; set; }

        public int? PersonalRating { get; set; }

        public DateOnly? StartedAt { get; set; }

        public DateOnly? CompletedAt { get; set; }

        public DateTime? LastPlayedAt { get; set; }

        public string? PrivateNotes { get; set; }


        // Navigations:

        // Ref N-1 ApplicationUser
        // Muchos registros de biblioteca pertenecen a un usuario.
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;


        // Ref N-1 Game
        // Muchos registros de biblioteca pueden apuntar al mismo juego.
        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;
    }
}
