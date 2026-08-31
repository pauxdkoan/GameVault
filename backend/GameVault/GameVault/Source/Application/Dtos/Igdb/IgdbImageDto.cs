using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb
{
    public sealed class IgdbImageDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("image_id")]
        public string ImageId { get; set; } = string.Empty;
    }
}
