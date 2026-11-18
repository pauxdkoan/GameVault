using GameVault.Source.Application.Dtos.Game.AdditionalDtos;
using GameVault.Source.Application.Dtos.Library;

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

        public List<PlatformDto> Platforms { get; set; } = [];


        //TODO: AGREGAR KEYWORDS y AGERatings
        public List<GameVideoDto>? Videos { get; set; }

        public List<string>? Screenshots { get; set; } = [];



        public List<GameCardDto> Dlcs { get; set; } = [];

        public List<GameCardDto> Expansions { get; set; } = [];



        public List<GameCardDto> ExpandedGames { get; set; } = [];



        public List<GameStoreLinkDto>? Stores { get; set; } = [];

        public List<string> GameModes { get; set; } = [];

        public List<InvolvedCompaniesDto> Companies {  get; set; } = [];

        public List<GameCardDto> SimilarGames {  get; set; } = [];

        public string StoryLine {  get; set; }= string.Empty;

        public List<WebSiteDto>? WebSites { get; set; } = [];

        public string? ParentGame { get; set; } = string.Empty;

        public string? GameType { get; set; } = string.Empty;


        // Datos de GameVault (no vienen de IGDB):

        /// <summary>Media de las reseñas de la comunidad (1 a 10). null si no hay reseñas.</summary>
        public decimal? CommunityRating { get; set; }

        public int ReviewCount { get; set; }

        /// <summary>
        /// El juego en la biblioteca del usuario que lo pide, para pintar el botón
        /// "Añadir a mi biblioteca" con el estado correcto. null sin sesión o si no lo tiene.
        /// </summary>
        public LibraryEntryDto? MyLibraryEntry { get; set; }

    }
}
