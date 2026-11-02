using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb.AdttionalDtos
{
    public sealed class IgdbInvolvedCompaniesDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("company")]
        public IgdbCompanyDto? CompanyDto { get; set; }


        [JsonPropertyName("developer")]
        public bool Developer { get; set; } 

        [JsonPropertyName("publisher")]
        public bool Publisher { get; set; } 

    }
}
