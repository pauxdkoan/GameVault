namespace GameVault.Source.Application.Dtos.Igdb
{
    public sealed class IgdbGamesResponse
    {
        public int Count { get; set; }

        public List<IgdbGameDto> Results { get; set; } = [];
    }
}
