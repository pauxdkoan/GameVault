using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb
{
    public sealed class IgdbGameDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDate { get; set; }

        [JsonPropertyName("cover")]
        public IgdbImageDto? Cover { get; set; }

        [JsonPropertyName("screenshots")]
        public List<IgdbImageDto> Screenshots { get; set; } = [];

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("aggregated_rating")]
        public double? AggregatedRating { get; set; }

        [JsonPropertyName("total_rating")]
        public double? TotalRating { get; set; }

        [JsonPropertyName("genres")]
        public List<IgdbGenreDto> Genres { get; set; } = [];

        [JsonPropertyName("platforms")]
        public List<IgdbPlatformDto> Platforms { get; set; } = [];
    }
}
