using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb.AdttionalDtos
{
    public sealed class IgdbWebsitesDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }


        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("type")]

        public IgdbTypeDto? Type {  get; set; }

    }
}
