namespace GameVault.Source.Domain.Settings
{
    public class IgdbSettings
    {
        public const string SectionName = "IgdbSettings";

        public string BaseUrl { get; set; } = string.Empty;

        public string TokenUrl { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;
    }
}
