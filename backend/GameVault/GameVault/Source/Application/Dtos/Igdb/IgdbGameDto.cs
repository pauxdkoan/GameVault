using GameVault.Source.Application.Dtos.Igdb.AdttionalDtos;
using System.Text.Json.Serialization;

namespace GameVault.Source.Application.Dtos.Igdb
{
    public sealed class IgdbGameDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDate { get; set; }

       


        [JsonPropertyName("cover")]
        public IgdbCoverDto? Cover { get; set; }

      

        [JsonPropertyName("artworks")]
        public List<IgdbCoverDto> Artworks { get; set; } = [];

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("aggregated_rating")]
        public double? AggregatedRating { get; set; }

        [JsonPropertyName("total_rating")]
        public double? TotalRating { get; set; }

        [JsonPropertyName("genres")]
        public List<IgdbGenreDto> Genres { get; set; } = [];

        [JsonPropertyName("platforms")]
        public List<IgdbPlatformDto> Platforms { get; set; } = [];

        [JsonPropertyName("themes")]
        public List<IgdbThemesDto> Themes { get; set; } = [];


        //Details Properties

        //TODO: Agregar el age_raitings...

        [JsonPropertyName("videos")]
        public List<IgdbVideoDto> Videos { get; set; } = [];

        [JsonPropertyName("screenshots")]
        public List<IgdbCoverDto> Screenshots { get; set; } = [];

        [JsonPropertyName("dlcs")]
        public List<IgdbDlcsDto> Dlcs { get; set; } = [];


        [JsonPropertyName("expansions")]
        public List<IgdbExpansionDto> Expansions { get; set; } = [];

        [JsonPropertyName("expanded_games")]
        public List<IgdbExpansionDto> ExpandedGames { get; set; } = [];

        [JsonPropertyName("external_games")]
        public List<IgdbExternalGameDto> ExternalGames { get; set; } = [];

        [JsonPropertyName("game_modes")]
        public List<IgdbGameModeDto> GameModes { get; set; } = [];

        [JsonPropertyName("involved_companies")]
        public List<IgdbInvolvedCompaniesDto> InvolvedCompanies { get; set; } = [];

        [JsonPropertyName("keywords")]
        public List<IgdbKeywordsDto> Keywords { get; set; } = [];

        [JsonPropertyName("similar_games")]
        public List<IgdbSimilarGamesDto> SimilarGames { get; set; } = [];

        [JsonPropertyName("storyline")]
        public string StoryLine { get; set; }= string.Empty;

        [JsonPropertyName("parent_game")]
        public IgdbParentGameDto? ParentGame { get; set; }

        [JsonPropertyName("websites")]
        public List<IgdbWebsitesDto> Websites { get; set; } = [];

        [JsonPropertyName("game_type")]
        public IgdbGameTypeDto? GameType { get; set; }

        [JsonPropertyName("age_ratings")]
        public List<IgdbAgeRatingDto> AgeRatings { get; set; } = [];


    }
}
