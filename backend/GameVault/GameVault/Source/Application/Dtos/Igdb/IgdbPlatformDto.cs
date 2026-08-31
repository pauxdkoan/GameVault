using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb
{
    public sealed class IgdbPlatformDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;
    }
}
