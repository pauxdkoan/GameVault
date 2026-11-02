using GameVault.Source.Application.Dtos.Game.AdditionalDtos;

namespace GameVault.Source.Application.Dtos.Game
{
    public sealed class GameDetailsDto 

    {
        public int ExternalId { get; set; }

        public string? Summary { get; set; }

        public string Slug { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DateOnly? Released { get; set; }

        public string? CoverImage { get; set; }

        public decimal? Rating { get; set; }

        public int? Metacritic { get; set; }

        public List<string> Themes { get; set; } = [];

        public List<string> Genres { get; set; } = [];

        public List<string> Platforms { get; set; } = [];


        //TODO: AGREGAR KEYWORDS y AGERatings
        public List<GameVideoDto>? Videos { get; set; }

        public List<string>? Screenshots { get; set; } = [];



        public List<RelatedGameDto>? Dlcs { get; set; } = [];

        public List<RelatedGameDto>? Expansions { get; set; } = [];



        public List<RelatedGameDto>? ExpandedGames { get; set; } = [];



        public List<GameStoreLinkDto>? Stores { get; set; } = [];

        public List<string> GameModes { get; set; } = [];

        public List<InvolvedCompaniesDto> Companies {  get; set; } = [];

        public List<RelatedGameDto>? SimilarGames {  get; set; } = [];

        public string StoryLine {  get; set; }= string.Empty;

        public List<WebSiteDto>? WebSites { get; set; } = [];

        public string? ParentGame { get; set; } = string.Empty;

        public string? GameType { get; set; } = string.Empty;

    }
}
