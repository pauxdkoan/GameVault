using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb.AdttionalDtos
{
    public sealed class IgdbCompanyDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("logo")]
        public IgdbLogoDto? Logo { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
