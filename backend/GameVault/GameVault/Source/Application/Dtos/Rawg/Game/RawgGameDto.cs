using GameVault.Source.Application.Dtos.Rawg.Genre;
using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Rawg.Game
{
    public sealed class RawgGameDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("released")]
        public DateOnly? Released { get; set; }

        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonPropertyName("rating")]
        public decimal Rating { get; set; }

        [JsonPropertyName("metacritic")]
        public int? Metacritic { get; set; }

        [JsonPropertyName("genres")]
        public List<RawgGenreDto> Genres { get; set; } = [];
    }
}
