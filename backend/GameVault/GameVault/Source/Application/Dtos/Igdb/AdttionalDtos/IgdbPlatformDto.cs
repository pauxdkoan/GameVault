using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb.AdttionalDtos
{
    public sealed class IgdbPlatformDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;



        [JsonPropertyName("platform_family")]
        public IgdbPlatformFamilyDto? PlatformFamily { get; set; }



        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;
    }
}
