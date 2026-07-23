using GameVault.Source.Domain.Common;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace GameVault.Source.Domain.Entities
{
    public class Game:BaseEntity
    {
        // Identificador del juego en RAWG.
        public int ExternalId { get; set; }

        public string Slug { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? CoverUrl { get; set; }

        public string? BackgroundImageUrl { get; set; }

        public DateOnly? ReleaseDate { get; set; }

        public decimal? ExternalRating { get; set; }

        public int? MetacriticScore { get; set; }

        public DateTime? LastSynchronizedAt { get; set; }


        // Navigations:

        // Ref 1-N UserGame
        public ICollection<UserGame> UserGames { get; set; } = [];


        // Ref 1-N Review
        public ICollection<Review> Reviews { get; set; } = [];

        // Ref 1-N GameListItem
        public ICollection<GameListItem> GameListItems { get; set; } = [];


        // Ref N-N Genre mediante GameGenre
        public ICollection<GameGenre> GameGenres { get; set; } = [];


        // Ref N-N Platform mediante GamePlatform
        public ICollection<GamePlatform> GamePlatforms { get; set; } = [];


       


    }
}
