using GameVault.Source.Application.Enums;

namespace GameVault.Source.Application.Dtos.Game
{
    /// <summary>
    /// Datos de un juego para pintarlo como tarjeta.
    /// Es el mismo DTO para el catálogo, tendencias, próximos lanzamientos,
    /// juegos similares, DLCs y expansiones. Los juegos relacionados
    /// traen menos datos de IGDB, así que algunos campos pueden venir vacíos.
    /// </summary>
    public sealed class GameCardDto
    {
        public int ExternalId { get; set; }

        public string Slug { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public DateOnly? Released { get; set; }

        public string? CoverImage { get; set; }

        public List<string> Screenshots { get; set; } = [];

        public string? TrailerVideo { get; set; }

        /// <summary>Nota de IGDB convertida a escala de 0 a 5.</summary>
        public decimal? Rating { get; set; }

        public int? Metacritic { get; set; }

        public List<string> Genres { get; set; } = [];

        /// <summary>Familias de plataforma sin repetir (PC, PlayStation...).</summary>
        public List<PlatformFamily> Platforms { get; set; } = [];
    }
}
