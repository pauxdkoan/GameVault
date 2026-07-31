namespace GameVault.Source.Application.Dtos.Game
{
    public sealed class GamesPageDto
    {
        public int Count { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public List<GameSummaryDto> Games { get; set; } = [];
    }
}
