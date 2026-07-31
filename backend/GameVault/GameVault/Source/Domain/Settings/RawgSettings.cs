namespace GameVault.Source.Domain.Settings
{
    public class RawgSettings
    {
        public const string SectionName = "RawgSettings";
        public string BaseUrl {  get; set; }
        public string ApiKey { get; set; }

    }
}
