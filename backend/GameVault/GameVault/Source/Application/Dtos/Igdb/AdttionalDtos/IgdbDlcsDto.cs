using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb.AdttionalDtos
{
    public sealed class IgdbDlcsDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

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
