namespace GameVault.Source.Application.Dtos.Game
{
    public sealed class GameSummaryDto
    {
        public int ExternalId { get; set; }

        public string? Summary { get; set; }



        public string Slug { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DateOnly? Released { get; set; }

        public string? Screenshot { get; set; }

        public string? CoverImage { get; set; }

        public decimal Rating { get; set; }

        public int? Metacritic { get; set; }

        public List<string> Genres { get; set; } = [];

        public List<string> Platforms { get; set; } = [];
    }
}
