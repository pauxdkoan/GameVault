using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb.AdttionalDtos
{
    public sealed class IgdbAgeRatingDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("organization")]
        public IgdbAgeRatingOrganizationDto? Organization { get; set; }

        [JsonPropertyName("rating_category")]
        public IgdbAgeRatingCategoryDto? RatingCategory { get; set; }

        [JsonPropertyName("rating_cover_url")]
        public string RatingCoverUrl { get; set; } = string.Empty;
    }
}
