using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Rawg.Game
{
    public sealed class RawgGamesResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("previous")]
        public string? Previous { get; set; }

        [JsonPropertyName("results")]
        public List<RawgGameDto> Results { get; set; } = [];
    }
}
