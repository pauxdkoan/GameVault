using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb.AdttionalDtos
{
    /// <summary>
    /// Juego relacionado tal como lo devuelve IGDB (DLC, expansión,
    /// juego expandido o similar). Antes había tres clases idénticas
    /// (IgdbDlcsDto, IgdbExpansionDto, IgdbSimilarGamesDto); ahora es una.
    /// </summary>
    public sealed class IgdbRelatedGameDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDate { get; set; }

        [JsonPropertyName("total_rating")]
        public double? TotalRating { get; set; }

        [JsonPropertyName("genres")]
        public List<IgdbGenreDto> Genres { get; set; } = [];

        [JsonPropertyName("platforms")]
        public List<IgdbPlatformDto> Platforms { get; set; } = [];

        [JsonPropertyName("cover")]
        public IgdbCoverDto? Cover { get; set; }
    }
}
