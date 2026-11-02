namespace GameVault.Source.Application.Dtos.Game.AdditionalDtos
{
    public sealed class RelatedGameDto
    {
        public int ExternalId {  get; set; }
        public string Name { get; set; }= string.Empty;

        public string? CoverImage {  get; set; }= string.Empty;

        public decimal? Raiting {  get; set; }

        public List<string> Platforms { get; set; } = [];
        public List<string> Genres { get; set; } = [];
    }
}
